using System;
using SFML.System;
using SFML.Window;

namespace TankGame.Actors.Buttons;

public class ActionTextButton : TextButton {
    public ActionTextButton(Vector2f position, Vector2f size, string text, uint fontSize, Action<MouseButtonEventArgs> action) : base(position, size, text, fontSize)
        => Action = action;

    protected Action<MouseButtonEventArgs> Action { get; }

    public override bool OnClick(MouseButtonEventArgs eventArgs) {
        Action.Invoke(eventArgs);
        return true;
    }
}