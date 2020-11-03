using SFML.Graphics;
using SFML.System;
using TankGame.Src.Actors;

namespace TankGame.Src.Gui.RenderComponents
{
    internal class RectangleComponent : IRenderComponent
    {
        private IRenderable Actor { get; }
        private RectangleShape Rectangle { get; }

        public RectangleComponent(Vector2f position, Vector2f size, IRenderable actor) : this(position, size, actor, new Color(0, 0, 0, 0))
        {
        }

        public RectangleComponent(Vector2f position, Vector2f size, IRenderable actor, Color fillColor) : this(position, size, actor, fillColor, new Color(), 0)
        {
        }

        public RectangleComponent(Vector2f position, Vector2f size, IRenderable actor, Color fillColor, Color outlineColor, float outlineThickness)
        {
            Rectangle = new RectangleShape(size)
            {
                Position = position,
                FillColor = fillColor,
                OutlineColor = outlineColor,
                OutlineThickness = outlineThickness
            };

            Actor = actor;
        }

        public Drawable GetShape()
        {
            return Rectangle;
        }

        public IRenderable GetActor()
        {
            return Actor;
        }

        public bool IsPointInside(Vector2f point)
        {
            if (Rectangle != null)
            {
                Vector2f position = Rectangle.Position;
                Vector2f size = Rectangle.Size;

                return (position.X <= point.X) && (position.X + size.X >= point.X) && (position.Y <= point.Y) && (position.Y + size.Y >= point.Y);
            }
            return false;
        }

        public void SetPosition(Vector2f position)
        {
            if (Rectangle != null)
            {
                Rectangle.Position = position;
            }
        }

        public void SetSize(Vector2f size)
        {
            if (Rectangle != null)
            {
                Rectangle.Size = size;
            }
        }
    }
}