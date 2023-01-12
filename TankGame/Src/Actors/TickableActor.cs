using System;
using SFML.System;

namespace TankGame.Actors;

public abstract class TickableActor : Actor, ITickable {
    protected TickableActor(Vector2f position, Vector2f size) : base(position, size)
        => (this as ITickable).RegisterTickable();

    public abstract void Tick(float deltaTime);

    public override void Dispose() {
        base.Dispose();
        GC.SuppressFinalize(this);
        (this as ITickable).UnregisterTickable();
    }
}