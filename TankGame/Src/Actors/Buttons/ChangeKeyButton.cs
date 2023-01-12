using System;
using System.Collections.Generic;
using SFML.System;
using SFML.Window;
using TankGame.Core.Controls;
using TankGame.Events;
using TankGame.Gui.RenderComponents;
using Action = TankGame.Core.Controls.Action;

namespace TankGame.Actors.Buttons;

public class ChangeKeyButton : TextButton {
    public ChangeKeyButton(Vector2f position, Vector2f size, Action action, uint fontSize, TextPosition horizontalPosition = TextPosition.Middle, TextPosition verticalPosition = TextPosition.Middle) : base(
        position, size, KeyManager.GetKey(action)
                                  .ToString(), fontSize, horizontalPosition, verticalPosition
    ) {
        Focused = false;
        BlinkTimer = 0;
        Action = action;

        MessageBus.CancelInputs += OnCancelInputs;
        MessageBus.KeyPressed += OnKeyPressed;
        MessageBus.MenuRefreshKeys += OnMenuRefreshKeys;
    }

    private bool Focused { get; set; }
    private byte BlinkTimer { get; set; }
    private Action Action { get; }

    public override HashSet<IRenderComponent> RenderComponents {
        get {
            if (!Focused) return base.RenderComponents;

            BlinkTimer++;
            ButtonText.SetText(BlinkTimer % 64 > 32 ? "" : "Press new key");

            return base.RenderComponents;
        }
    }

    private void OnMenuRefreshKeys()
        => ButtonText.SetText(
            KeyManager.GetKey(Action)
                      .ToString()
        );

    private void OnKeyPressed(KeyEventArgs eventArgs) {
        if (!Focused) return;

        KeyManager.ChangeAndSaveKey(new(eventArgs.Code, Action));
        Focused = false;

        MessageBus.MenuRefreshKeys.Invoke();
    }

    private void OnCancelInputs() {
        Focused = false;
        ButtonText.SetText(
            KeyManager.GetKey(Action)
                      .ToString()
        );
    }

    public override bool OnClick(MouseButtonEventArgs eventArgs) {
        MessageBus.CancelInputs.Invoke();
        return Focused = true;
    }

    public override void Dispose() {
        GC.SuppressFinalize(this);

        MessageBus.CancelInputs -= OnCancelInputs;
        MessageBus.KeyPressed -= OnKeyPressed;
        MessageBus.MenuRefreshKeys -= OnMenuRefreshKeys;
    }
}