using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Actors;
using TankGame.Src.Data;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Events
{
    internal class InputHandler
    {
        private HashSet<IClickable> Clickables { get; }
        private RenderWindow Window { get; }

        public InputHandler(RenderWindow window)
        {
            Clickables = new HashSet<IClickable>();
            Window = window;

            RegisterEvents();
        }

        public void OnKeyPress(object sender, KeyEventArgs eventArgs)
        {
            Tuple<string, string> keyActionType = KeyManager.Instance.GetAction(eventArgs.Code);
            if (keyActionType != null) MessageBus.Instance.PostEvent(MessageType.KeyAction, sender, new KeyActionEventArgs(keyActionType));
            MessageBus.Instance.PostEvent(MessageType.KeyPressed, sender, eventArgs);
        }
        public void OnTextInput(object sender, TextEventArgs textEventArgs)
        {
            MessageBus.Instance.PostEvent(MessageType.TextInput, sender, textEventArgs);
        }

        public void OnClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Vector2f point = Window.MapPixelToCoords(new Vector2i(mouseButtonEventArgs.X, mouseButtonEventArgs.Y));

            Clickables.ToList()
                .FindAll(clickable => clickable.Visible)
                .ForEach(clickable 
                    => clickable.GetRenderComponents()
                        .ToList()
                        .FindAll(component => component != null && component.IsPointInside(point))
                        .ForEach(_ => clickable.OnClick(mouseButtonEventArgs)));
        }

        private void RegisterEvents()
        {
            MessageBus messageBus = MessageBus.Instance;

            messageBus.Register(MessageType.RegisterClickable, OnRegisterClickable);
            messageBus.Register(MessageType.UnregisterClickable, OnUnregisterClickable);
        }

        private void OnRegisterClickable(object sender, EventArgs eventArgs)
        {
            if (sender is IClickable) Clickables.Add((IClickable)sender);
        }

        private void OnUnregisterClickable(object sender, EventArgs eventArgs)
        {
            if (sender is IClickable && Clickables.Contains((IClickable)sender)) Clickables.Remove((IClickable)sender);
        }
    }
}