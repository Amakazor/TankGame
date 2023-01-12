using SFML.Graphics;
using SFML.System;

namespace TankGame.Gui.RenderComponents;

public class RectangleComponent : IRenderComponent {
    public RectangleComponent(Vector2f position, Vector2f size, Color fillColor = new(), Color outlineColor = new(), float outlineThickness = 0)
        => Rectangle = new(size) {
            Position = position, FillColor = fillColor, OutlineColor = outlineColor, OutlineThickness = outlineThickness,
        };

    private RectangleShape Rectangle { get; }

    public Drawable Shape => Rectangle;

    public void SetPosition(Vector2f position)
        => Rectangle.Position = position;

    public void SetSize(Vector2f size)
        => Rectangle.Size = size;

    public bool IsPointInside(Vector2f point) {
        Vector2f position = Rectangle.Position;
        Vector2f size = Rectangle.Size;

        return position.X <= point.X && position.X + size.X >= point.X && position.Y <= point.Y && position.Y + size.Y >= point.Y;
    }

    public void SetFillColor(Color color)
        => Rectangle.FillColor = color;
}