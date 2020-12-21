using SFML.System;
using System.Collections.Generic;
using TankGame.Src.Actors.Pawn;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Projectiles
{
    internal abstract class Projectile : TickableActor
    {
        protected Direction Direction { get; }
        protected SpriteComponent ProjectileComponent { get; set; }
        protected RectangleComponent CollisionRectangle { get; set; }

        public Projectile(Vector2f position, Direction direction) : base(position, new Vector2f(64, 64))
        {
            Direction = direction;
            CollisionRectangle = new RectangleComponent(Position, Size, this);
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
