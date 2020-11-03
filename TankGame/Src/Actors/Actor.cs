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
        private Vector2f Position { get; }
        private Vector2f Size { get; }

        public Actor(Vector2f position, Vector2f size)
        {
            Position = position;
            Size = size;
        }

        public abstract HashSet<IRenderComponent> GetRenderComponents();

        public void RegisterRenderable(IRenderable renderable)
        {
            MessageBus.Instance.PostEvent(MessageType.RegisterRenderable, this, new EventArgs());
        }

        public void UnregisterRenderable(IRenderable renderable)
        {
            MessageBus.Instance.PostEvent(MessageType.UnregisterRenderable, this, new EventArgs());
        }

        public void Dispose()
        {
            UnregisterRenderable(this);
        }
    }
}
