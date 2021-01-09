using SFML.Graphics;
using SFML.System;
using TankGame.Src.Actors.Pawns;

namespace TankGame.Src.Gui.RenderComponents
{
    internal class SpriteComponent : IRenderComponent
    {
        private Sprite Sprite { get; }
        private Vector2f Size { get; set; }

        public SpriteComponent(Vector2f position, Vector2f size, Texture texture, Color color, Direction direction = Direction.Down)
        {
            Sprite = new Sprite(texture)
            {
                Position = position,
                Color = color
            };

            Size = size;

            SetScaleFromSize(size);
            SetDirection(direction);

            Sprite.Origin = size / 2;
        }

        private void SetScaleFromSize(Vector2f size)
        {
            Sprite.Scale = size.X != 0 && size.Y != 0 ? new Vector2f(size.X / 64, size.Y / 64) : new Vector2f(1, 1);
        }

        public bool IsPointInside(Vector2f point)
        {
            Vector2f position = Sprite.Position;
            Vector2f size = Size;

            return (position.X <= point.X) && (position.X + size.X >= point.X) && (position.Y <= point.Y) && (position.Y + size.Y >= point.Y);
        }

        public void SetSize(Vector2f size)
        {
            Size = size;
            SetScaleFromSize(size);
        }

        public void SetTextureRectSize(Vector2i size)
        {
            Sprite.TextureRect = new IntRect(new Vector2i(0, 0), size);
        }

        public void SetPosition(Vector2f position) => Sprite.Position = position;

        public void SetDirection(Direction direction)
        {
            Sprite.Rotation = direction switch
            {
                Direction.Up => 180,
                Direction.Down => 0,
                Direction.Left => 90,
                Direction.Right => 270,
                _ => 0
            };
        }

        public void SetDirection(double angle)
        {
            Sprite.Rotation = (float)angle;
        }

        Drawable IRenderComponent.Shape => Sprite;
    }
}