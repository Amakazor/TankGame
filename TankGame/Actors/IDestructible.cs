using TankGame.Events;

namespace TankGame.Actors;

public enum DestructabilityType {
    Destructible,
    Indestructible,
    DestroyOnEntry,
}

public interface IDestructible {
    public bool IsAlive => Health > 0;
    public int Health { get; set; } 
    public DestructabilityType DestructabilityType { get; }
    public bool StopsProjectile { get; }
    public Actor Actor { get; }

    public void Hit();
    public void Destroy();

    public void RegisterDestructible()
        => MessageBus.RegisterDestructible.Invoke(this);

    public void UnregisterDestructible()
        => MessageBus.UnregisterDestructible.Invoke(this);
}