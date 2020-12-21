using SFML.System;
using System;
using TankGame.Src.Events;

namespace TankGame.Src.Actors
{
    internal abstract class TickableActor : Actor, ITickable
    {
        public TickableActor(Vector2f position, Vector2f size) : base(position, size)
        {
            RegisterTickable();
        }

        public void RegisterTickable()
        {
            MessageBus.Instance.PostEvent(MessageType.RegisterTickable, this, new EventArgs());
        }

        public void UnregisterTickable()
        {
            MessageBus.Instance.PostEvent(MessageType.UnregisterTickable, this, new EventArgs());
        }

        public abstract void Tick(float deltaTime);

        public override void Dispose()
        {
            UnregisterTickable();
            base.Dispose();
        }
    }
}
