using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Actors.Data;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Data.Gamestate;
using TankGame.Src.Data.Sounds;
using TankGame.Src.Data.Textures;
using TankGame.Src.Events;
using TankGame.Src.Extensions;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Projectiles
{
    internal class Projectile : TickableActor
    {
        private const float BaseFlightDistance = 64 * 7;
        private const float BaseSpeed = 200;

        private readonly float FlightDistance = BaseFlightDistance * (1 / (GamestateManager.Instance.WeatherModifier * GamestateManager.Instance.WeatherModifier));
        private readonly float FlightSpeed = BaseSpeed * (1 / (GamestateManager.Instance.WeatherModifier * GamestateManager.Instance.WeatherModifier));
        private Direction Direction { get; }
        private SpriteComponent ProjectileComponent { get; set; }
        public Pawn Owner { get; private set; }
        private Vector2f StartingPosition { get; }
        private bool HasFlownToFar => StartingPosition.ManhattanDistance(Position) >= FlightDistance;
        private float SpeedMultiplier => (float)(-(Math.Pow(Math.Cos(Math.PI * FlightProgressReversed), (FlightProgressReversed > 0.5) ? 3 : 9) - 1) / 2 + 0.2);

        private double FlightProgressReversed => 1 - StartingPosition.ManhattanDistance(Position) / FlightDistance;
        public Vector2f CollisionPosition => Position + ((Size - CollistionSize) / 2);
        public Vector2f CollistionSize => (Size / 4);

        public Projectile(Vector2f position, Direction direction, Pawn owner) : base(position, new Vector2f(64, 64))
        {
            StartingPosition = Position;
            Direction = direction;
            Owner = owner;

            ProjectileComponent = Owner switch
            {
                Enemy _ => new SpriteComponent(Position, Size, TextureManager.Instance.GetTexture(TextureType.Projectile, "pocisk1"), new Color(255, 255, 255, 255), Direction),
                Player _ => new SpriteComponent(Position, Size, TextureManager.Instance.GetTexture(TextureType.Projectile, "pocisk2"), new Color(255, 255, 255, 255), Direction),
                _ => throw new System.NotImplementedException(),
            };

            MessageBus.Instance.PostEvent(MessageType.RegisterProjectile, this, new EventArgs());
            SoundManager.Instance.PlayRandomSound("shot", Position / 64);

            RenderLayer = RenderLayer.Projectile;
            RenderView = RenderView.Game;
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { ProjectileComponent };
        }

        public override void Tick(float deltaTime)
        {
            if (!HasFlownToFar && !(GamestateManager.Instance.Map != null && GamestateManager.Instance.Map.IsOutOfBounds(Position)))
            {
                Vector2f moveVector = Direction switch
                {
                    Direction.Up => new Vector2f(0, -FlightSpeed * deltaTime * SpeedMultiplier),
                    Direction.Down => new Vector2f(0, FlightSpeed * deltaTime * SpeedMultiplier),
                    Direction.Left => new Vector2f(-FlightSpeed * deltaTime * SpeedMultiplier, 0),
                    Direction.Right => new Vector2f(FlightSpeed * deltaTime * SpeedMultiplier, 0),
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