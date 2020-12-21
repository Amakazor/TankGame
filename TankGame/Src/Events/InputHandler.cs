using SFML.Graphics;
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
        private RenderWindow Window { get; set; }

        public InputHandler(RenderWindow window)
        {
            Clickables = new HashSet<IClickable>();
            Window = window;

            RegisterEvents();
        }

        public void OnKeyPress(object sender, KeyEventArgs eventArgs)
        {
            Console.WriteLine("Key pressed: " + eventArgs.Code);

            Tuple<string, string> keyActionType = KeyManager.Instance.GetAction(eventArgs.Code);
            if (keyActionType != null)
            {
                Console.WriteLine("Action done: " + keyActionType.Item2);
                MessageBus.Instance.PostEvent(MessageType.KeyAction, sender, new KeyActionEventArgs(keyActionType));
            }
            MessageBus.Instance.PostEvent(MessageType.KeyPressed, sender, eventArgs);
        }

        public void OnClick(object sender, MouseButtonEventArgs eventArgs)
        {
            Vector2f point = Window.MapPixelToCoords(new Vector2i(eventArgs.X, eventArgs.Y));

            Console.WriteLine("Mouse clicked at:");
            Console.WriteLine("\tScreen coordinates: " + eventArgs.X + " " + eventArgs.Y);
            Console.WriteLine("\tGlobal coordinates: " + point.X + " " + point.Y);

            foreach (IClickable clickable in Clickables)
            {
                foreach (IRenderComponent component in clickable.GetRenderComponents())
                {
                    if (component.IsPointInside(point))
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