using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors
{
    internal abstract class Actor : IRenderable, IDisposable
    {
        protected Vector2f Position { get; set; }
        protected Vector2f Size { get; set; }

        public Actor(Vector2f position, Vector2f size)
        {
            Position = position;
            Size = size;

            RegisterRenderable();
        }

        public abstract HashSet<IRenderComponent> GetRenderComponents();

        public void RegisterRenderable()
        {
            MessageBus.Instance.PostEvent(MessageType.RegisterRenderable, this, new EventArgs());
        }

        public void UnregisterRenderable()
        {
            MessageBus.Instance.PostEvent(MessageType.UnregisterRenderable, this, new EventArgs());
        }

        public virtual void Dispose()
        {
            UnregisterRenderable();
        }
    }
}
