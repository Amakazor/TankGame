using SFML.System;
using SFML.Window;
using System;

namespace TankGame.Src.Actors.Buttons
{
    internal class ActionTextButton : TextButton
    {
        protected Action<MouseButtonEventArgs> Action { get; }

        public ActionTextButton(Vector2f position, Vector2f size, string text, uint fontSize, Action<MouseButtonEventArgs> action) : base(position, size, text, fontSize)
        {
            Action = action;
        }

        public override bool OnClick(MouseButtonEventArgs eventArgs)
        {
            Action.Invoke(eventArgs);
            return true;
        }
    }
}