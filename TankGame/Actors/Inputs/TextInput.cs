using System;
using System.Collections.Generic;
using SFML.System;
using SFML.Window;
using TankGame.Events;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Buttons;

public class TextInput : Button {
    public TextInput(Vector2f position, Vector2f size, string placeholderText, uint fontSize, TextPosition horizontalPosition = TextPosition.Middle, TextPosition verticalPosition = TextPosition.Middle) : base(position, size) {
        Focused = false;

        InputText = new(position, size, new(0, 0), fontSize, horizontalPosition, verticalPosition, "", new(255, 255, 255, 255));
        InputPlaceholderText = new(position, size, new(0, 0), fontSize, horizontalPosition, verticalPosition, placeholderText, new(200, 200, 200, 255));
        BoundingBox = new(position, size, new(32, 32, 32, 255), new(128, 128, 128, 255), 2);

        MessageBus.CancelInputs += OnCancelInputs;
        MessageBus.TextInput += OnKeyPressed;
    }

    public string Text { get; private set; }

    protected AlignedTextComponent InputText { get; }
    protected AlignedTextComponent InputPlaceholderText { get; }
    private bool Focused { get; set; }

    public override HashSet<IRenderComponent> RenderComponents => new() { BoundingBox, string.IsNullOrEmpty(Text) ? InputPlaceholderText : null, InputText };

    private void OnKeyPressed(TextEventArgs eventArgs) {
        if (Focused) {
            if (eventArgs.Unicode == "\b" && Text.Length != 0)
                Text = Text.Remove(Text.Length - 1);
            else if (eventArgs.Unicode != "\x1B" && eventArgs.Unicode != "\t" && eventArgs.Unicode != "\b" && (Text == null || Text.Length < 10)) Text += eventArgs.Unicode;
        }

        if (Text != null) InputText.SetText(Text);
    }

    private void OnCancelInputs()
        => Focused = false;

    public override void Dispose() {
        base.Dispose();
        GC.SuppressFinalize(this);
        MessageBus.CancelInputs -= OnCancelInputs;
        MessageBus.TextInput -= OnKeyPressed;
    }

    public override bool OnClick(MouseButtonEventArgs eventArgs) {
        MessageBus.CancelInputs.Invoke();
        return Focused = true;
    }
}