using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Pawns;

namespace TankGame.Gui.RenderComponents;

public class SpriteComponent : IRenderComponent {
    public SpriteComponent(Vector2f position, Vector2f size, Texture texture, Color color, Direction direction = Direction.Down) {
        Sprite = new(texture) { Position = position, Color = color };

        Size = size;

        SetScaleFromSize(size);
        SetRotation(direction);

        Sprite.Origin = size / 2;
    }

    private Sprite Sprite { get; }
    private Vector2f Size { get; set; }

    public bool IsPointInside(Vector2f point) {
        Vector2f position = Sprite.Position;
        Vector2f size = Size;

        return position.X <= point.X && position.X + size.X >= point.X && position.Y <= point.Y && position.Y + size.Y >= point.Y;
    }

    public void SetSize(Vector2f size) {
        Size = size;
        SetScaleFromSize(size);
    }

    public void SetPosition(Vector2f position)
        => Sprite.Position = position;

    Drawable IRenderComponent.Shape => Sprite;

    private void SetScaleFromSize(Vector2f size)
        => Sprite.Scale = size.X != 0 && size.Y != 0 ? new(size.X / 64, size.Y / 64) : new Vector2f(1, 1);

    public void SetScale(Vector2f scale)
        => Sprite.Scale = scale;

    public void SetTextureRectSize(Vector2i size)
        => Sprite.TextureRect = new(new(0, 0), size);

    public void SetRotation(Direction direction)
        => SetRotation(
            direction switch {
                Direction.Up    => 180,
                Direction.Down  => 0,
                Direction.Left  => 90,
                Direction.Right => 270,
                _               => 0,
            }
        );

    public void SetRotation(double angle)
        => Sprite.Rotation = (float)angle;
}