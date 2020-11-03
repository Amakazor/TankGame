using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using TankGame.Src.Actors;
using TankGame.Src.Data;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Events
{
    internal class InputHandler
    {
        private HashSet<IClickable> Clickables { get; }

        public InputHandler()
        {
            Clickables = new HashSet<IClickable>();
            RegisterEvents();
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

        public void OnClick(object sender, MouseButtonEventArgs eventArgs)
        {
            foreach (IClickable clickable in Clickables)
            {
                foreach(IRenderComponent component in clickable.GetRenderComponents())
                {
                    if (component.IsPointInside(new Vector2f(eventArgs.X, eventArgs.Y)))
                    {
                        clickable.OnClick(eventArgs);
                    }
                }
            }
        }

        private void RegisterEvents()
        {
            MessageBus messageBus = MessageBus.Instance;

            messageBus.Register(MessageType.RegisterClickable, OnRegisterClickable);
            messageBus.Register(MessageType.UnregisterClickable, OnUnregisterClickable);
        }

        private void OnRegisterClickable(object sender, EventArgs eventArgs)
        {
            if (sender is IClickable)
            {
                Clickables.Add((IClickable)sender);
            }
        }
        private void OnUnregisterClickable(object sender, EventArgs eventArgs)
        {
            if (sender is IClickable && Clickables.Contains((IClickable)sender))
            {
                Clickables.Remove((IClickable)sender);
            }
        }
    }
}
