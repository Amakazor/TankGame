using SFML.Window;
using System;
using TankGame.Src.Data;

namespace TankGame.Src.Events
{
    internal class InputHandler
    {
        public InputHandler()
        {
        }

        public void OnKeyPress(object sender, KeyEventArgs eventArgs)
        {
            Tuple<string, string> keyActionType = KeyManager.Instance.GetAction(eventArgs.Code);
            if (keyActionType != null)
            {
                MessageBus.Instance.PostEvent(MessageType.KeyAction, sender, new KeyActionEventArgs(keyActionType));
            }
            MessageBus.Instance.PostEvent(MessageType.KeyPressed, sender, eventArgs);
        }
    }
}
