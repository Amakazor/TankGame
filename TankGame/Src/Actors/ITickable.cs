using TankGame.Events;

namespace TankGame.Actors;

public interface ITickable {
    public void Tick(float deltaTime);

    public void RegisterTickable()
        => MessageBus.RegisterTickable.Invoke(this);

    public void UnregisterTickable()
        => MessageBus.UnregisterTickable.Invoke(this);
}