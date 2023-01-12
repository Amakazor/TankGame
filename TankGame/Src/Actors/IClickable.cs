using SFML.Window;
using TankGame.Events;

namespace TankGame.Actors;

public interface IClickable : IRenderable {
    public bool OnClick(MouseButtonEventArgs eventArgs);

    public void RegisterClickable()
        => MessageBus.RegisterClickable.Invoke(this);

    public void UnregisterClickable()
        => MessageBus.UnregisterClickable.Invoke(this);
}