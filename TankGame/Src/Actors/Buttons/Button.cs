using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Buttons
{
    internal abstract class Button : Actor, IClickable
    {
        protected RectangleComponent BoundingBox { get; }

        public Button(Vector2f position, Vector2f size) : base(position, size)
        {
            BoundingBox = new RectangleComponent(Position, Size, this);

            RegisterClickable();
        }

        public abstract bool OnClick(MouseButtonEventArgs eventArgs);

        public void RegisterClickable()
        {
            MessageBus.Instance.PostEvent(MessageType.RegisterClickable, this, new EventArgs());
        }

        public void UnregisterClickable()
        {
            MessageBus.Instance.PostEvent(MessageType.UnregisterClickable, this, new EventArgs());
        }

        public override void Dispose()
        {
            base.Dispose();
            UnregisterClickable();
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { BoundingBox };
        }
    }
}