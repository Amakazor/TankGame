using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Actors.Data;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors
{
    internal abstract class Actor : IRenderable, IDisposable
    {
        public Vector2f Position { get; protected set; }
        public Vector2f Size { get; protected set; }
        protected RenderLayer RenderLayer { get; set; }
        protected RenderView RenderView { get; set; }
        public RenderLayer RenderableRenderLayer => RenderLayer;
        public RenderView RenderableRenderView => RenderView;
        public bool Visible { get; set; }

        protected Actor(Vector2f position, Vector2f size)
        {
            Position = position;
            Size = size;

            RegisterRenderable();
            Visible = true;
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