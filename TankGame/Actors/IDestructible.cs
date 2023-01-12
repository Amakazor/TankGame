using TankGame.Events;

namespace TankGame.Actors;

public interface IDestructible {
    public bool IsAlive { get; }
    public int CurrentHealth { get; set; }
    public bool IsDestructible { get; }
    public bool StopsProjectile { get; }
    public Actor Actor { get; }

    public void OnHit();
    public void OnDestroy();

    public void RegisterDestructible()
        => MessageBus.RegisterDestructible.Invoke(this);

    public void UnregisterDestructible()
        => MessageBus.UnregisterDestructible.Invoke(this);
}