using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TankGame.Actors;
using TankGame.Core.Controls;

namespace TankGame.Events;

public class InputHandler {
    public InputHandler(RenderWindow window) {
        Clickables = new();
        Window = window;

        RegisterEvents();
    }

    private HashSet<IClickable> Clickables { get; }
    private RenderWindow Window { get; }

    public void OnKeyPress(KeyEventArgs eventArgs) {
        MessageBus.Action.Invoke(KeyManager.GetAction(eventArgs.Code));
        MessageBus.KeyPressed.Invoke(eventArgs);
    }

    public void OnTextInput(TextEventArgs textEventArgs)
        => MessageBus.TextInput.Invoke(textEventArgs);

    public void OnClick(MouseButtonEventArgs mouseButtonEventArgs) {
        Vector2f point = Window.MapPixelToCoords(new(mouseButtonEventArgs.X, mouseButtonEventArgs.Y));

        Clickables.ToList()
                  .FindAll(clickable => clickable.Visible)
                  .ForEach(
                       clickable => clickable.RenderComponents.ToList()
                                             .FindAll(component => component != null && component.IsPointInside(point))
                                             .ForEach(_ => clickable.OnClick(mouseButtonEventArgs))
                   );
    }

    private void RegisterEvents() {
        MessageBus.RegisterClickable += sender => Clickables.Add(sender);
        MessageBus.UnregisterClickable += sender => Clickables.Remove(sender);
    }
}