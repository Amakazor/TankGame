using SFML.Graphics;
using SFML.System;

namespace TankGame.Src.Gui.RenderComponents
{
    internal class RectangleComponent : IRenderComponent
    {
        private RectangleShape Rectangle { get; }

        public RectangleComponent(Vector2f position, Vector2f size) : this(position, size, new Color(0, 0, 0, 0))
        {
        }

        public RectangleComponent(Vector2f position, Vector2f size, Color fillColor) : this(position, size, fillColor, new Color(), 0)
        {
        }

        public RectangleComponent(Vector2f position, Vector2f size, Color fillColor, Color outlineColor, float outlineThickness)
        {
            Rectangle = new RectangleShape(size)
            {
                Position = position,
                FillColor = fillColor,
                OutlineColor = outlineColor,
                OutlineThickness = outlineThickness
            };
        }

        public Drawable Shape => Rectangle;
        public void SetPosition(Vector2f position) => Rectangle.Position = position;
        public void SetSize(Vector2f size) => Rectangle.Size = size;
        public void SetFillColor(Color color) => Rectangle.FillColor = color;

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
    }
}