using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Data;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Projectiles
{
    internal class Projectile : TickableActor
    {
        protected Direction Direction { get; }
        protected SpriteComponent ProjectileComponent { get; set; }
        protected RectangleComponent CollisionRectangle { get; set; }
        protected Pawn Owner { get; }

        public Projectile(Vector2f position, Direction direction, Pawn owner) : base(position, new Vector2f(64, 64))
        {
            Direction = direction;
            Owner = owner;
            CollisionRectangle = new RectangleComponent(Position, Size, this);

            ProjectileComponent = Owner switch
            {
                Enemy  _ => new SpriteComponent(Position, Size, this, TextureManager.Instance.GetTexture(TextureType.Projectile, "pocisk1"), new Color(255, 255, 255, 255), Direction),
                Player _ => new SpriteComponent(Position, Size, this, TextureManager.Instance.GetTexture(TextureType.Projectile, "pocisk2"), new Color(255, 255, 255, 255), Direction),
                _ => throw new System.NotImplementedException(),
            };
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { ProjectileComponent, CollisionRectangle };
        }

        public override void Tick(float deltaTime)
        {
            Vector2f moveVector = Direction switch
            {
                Direction.Up => new Vector2f(0 * deltaTime, -300 * deltaTime),
                Direction.Down => new Vector2f(0 * deltaTime, 300 * deltaTime),
                Direction.Left => new Vector2f(-300 * deltaTime, 0 * deltaTime),
                Direction.Right => new Vector2f(300 * deltaTime, 0 * deltaTime),
                _ => new Vector2f(0, 0),
            };

            Position += moveVector;
            ProjectileComponent.SetPosition(Position);
            CollisionRectangle.SetPosition(Position);
        }
    }
}
