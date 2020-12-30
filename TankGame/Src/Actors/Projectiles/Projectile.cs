using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Data;
using TankGame.Src.Events;
using TankGame.Src.Extensions;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Projectiles
{
    internal class Projectile : TickableActor
    {
        private static readonly float FlightDistance = 640;
        private Direction Direction { get; }
        private SpriteComponent ProjectileComponent { get; set; }
        public Pawn Owner { get; private set; }
        private Vector2f StartingPosition { get; }
        private bool HasFlownToFar => StartingPosition.ManhattanDistance(Position) >= FlightDistance;
        public Vector2f CollisionPosition => Position + (Size / 4);
        public Vector2f CollistionSize => (Size / 2);

        public Projectile(Vector2f position, Direction direction, Pawn owner) : base(position, new Vector2f(64, 64))
        {
            StartingPosition = Position;
            Direction = direction;
            Owner = owner;

            ProjectileComponent = Owner switch
            {
                Enemy _ => new SpriteComponent(Position, Size, this, TextureManager.Instance.GetTexture(TextureType.Projectile, "pocisk1"), new Color(255, 255, 255, 255), Direction),
                Player _ => new SpriteComponent(Position, Size, this, TextureManager.Instance.GetTexture(TextureType.Projectile, "pocisk2"), new Color(255, 255, 255, 255), Direction),
                _ => throw new System.NotImplementedException(),
            };

            MessageBus.Instance.PostEvent(MessageType.RegisterProjectile, this, new EventArgs());
            SoundManager.Instance.PlayRandomSound("shot", Position / 64);
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { ProjectileComponent };
        }

        public override void Tick(float deltaTime)
        {
            if (!HasFlownToFar)
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
            }
            else Dispose();
        } 
            
        public override void Dispose()
        {
            Owner = null;
            MessageBus.Instance.PostEvent(MessageType.UnregisterProjectile, this, new EventArgs());
            base.Dispose();
        }
    }
}
