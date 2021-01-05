using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using TankGame.Src.Data;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Buttons
{
    internal class ChangeKeyButton : TextButton
    {
        private bool IsWaitingForKey { get; set; }
        private byte Blink { get; set; }
        private Tuple<string, string> KeyActionType { get; }

        public ChangeKeyButton(Vector2f position, Vector2f size, Tuple<string, string> keyActionType, uint fontSize, TextPosition horizontalPosition = TextPosition.Middle, TextPosition verticalPosition = TextPosition.Middle) : base(position, size, KeyManager.Instance.GetKey(keyActionType).ToString(), fontSize, horizontalPosition, verticalPosition)
        {
            IsWaitingForKey = false;
            Blink = 0;
            KeyActionType = keyActionType;

            MessageBus.Instance.Register(MessageType.CancelKeyChanges, OnCancelKeyChanges);
            MessageBus.Instance.Register(MessageType.KeyPressed, OnKeyPressed);
            MessageBus.Instance.Register(MessageType.MenuRefreshKeys, OnMenuRefreshKeys);
        }

        private void OnMenuRefreshKeys(object sender, EventArgs eventArgs)
        {
            ButtonText.SetText(KeyManager.Instance.GetKey(KeyActionType).ToString());
        }

        private void OnKeyPressed(object sender, EventArgs eventArgs)
        {
            if (IsWaitingForKey && eventArgs is KeyEventArgs keyEventArgs)
            {
                KeyManager.Instance.ChangeAndSaveKey(KeyActionType, keyEventArgs.Code);
                IsWaitingForKey = false;

                MessageBus.Instance.PostEvent(MessageType.MenuRefreshKeys, this, new EventArgs());
            }
        }

        private void OnCancelKeyChanges(object sender, EventArgs eventArgs)
        {
            IsWaitingForKey = false;
            ButtonText.SetText(KeyManager.Instance.GetKey(KeyActionType).ToString());
        }

        public override bool OnClick(MouseButtonEventArgs eventArgs)
        {
            MessageBus.Instance.PostEvent(MessageType.CancelKeyChanges, this, new EventArgs());
            return IsWaitingForKey = true;
        }

        public override void Dispose()
        {
            MessageBus.Instance.Unregister(MessageType.CancelKeyChanges, OnCancelKeyChanges);
            MessageBus.Instance.Unregister(MessageType.KeyPressed, OnKeyPressed);
            MessageBus.Instance.Unregister(MessageType.MenuRefreshKeys, OnMenuRefreshKeys);

        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            if (IsWaitingForKey)
            {
                Blink++;
                if (Blink % 64 > 32) ButtonText.SetText("");
                else ButtonText.SetText("Press new key");
            }
            return base.GetRenderComponents();
        }
    }
}
