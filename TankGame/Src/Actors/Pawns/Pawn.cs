using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Actors.Pawns.MovementControllers;
using TankGame.Src.Data;
using TankGame.Src.Data.Map;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Pawns
{
    internal abstract class Pawn : TickableActor, IDestructible
    {
        public Direction LastDirection { get; set; }
        public Direction Direction { get; set; }
        public int Health { get; set; }
        public MovementController MovementController { get; set; }
        private Texture Texture { get; }
        private SpriteComponent PawnSprite { get; }
        public Vector2f LastPosition { get; set; }
        public Vector2f RealPosition => CalculatePosition();
        public Vector2i Coords
        {
            get => new Vector2i((int)(Position.X / Size.X), (int)(Position.Y / Size.Y));
            set => Position = new Vector2f(value.X * Size.X, value.Y * Size.Y);
        }
        public Vector2i LastCoords => new Vector2i((int)(LastPosition.X / Size.X), (int)(LastPosition.Y / Size.Y));
        public bool IsAlive => Health > 0;
        public bool IsDestructible => true;
        public Actor Actor => this;
        public Region CurrentRegion => GamestateManager.Instance.Map.GetRegionFromFieldCoords(Coords);

        public Pawn(Vector2f position, Vector2f size, Texture texture, int health) : base(position, size)
        {
            Health = health;
            PawnSprite = new SpriteComponent(Position, Size, this, Texture = texture, new Color(255, 255, 255, 255));
            PawnSprite.SetDirection(Direction.Up);
            RegisterDestructible();

            RenderLayer = RenderLayer.Pawn;
            RenderView = RenderView.Game;

        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { PawnSprite };
        }

        public override void Tick(float deltaTime)
        {
            if (MovementController.IsRotating) PawnSprite.SetDirection(CalculateRotationAngle());
            if (MovementController.IsMoving) PawnSprite.SetPosition(RealPosition);

            if (MovementController != null)
            {
                if (MovementController.CanDoAction())
                {
                    Vector2i lastCoords = Coords;
                    Direction = MovementController.DoAction(Direction);
                    Vector2i newCoords = Coords;
                    UpdatePosition(lastCoords, newCoords);
                }
                else if (MovementController is PlayerMovementController) MovementController.ClearAction();

                MovementController.Tick(deltaTime);
            }
        }

        protected virtual void UpdatePosition(Vector2i lastCoords, Vector2i newCoords)
        {
            PawnSprite.SetPosition(RealPosition);
            PawnSprite.SetDirection(CalculateRotationAngle());
            MessageBus.Instance.PostEvent(MessageType.PawnMoved, this, new PawnMovedEventArgs(lastCoords, newCoords));
        }

        protected double CalculateRotationAngle()
        {
            if (!MovementController.IsRotating) return GetRotationAngleFromDirection(Direction);
            else
            {
                double startRotationAngle = GetRotationAngleFromDirection(LastDirection);
                double endRotationAngle = GetRotationAngleFromDirection(Direction);

                if (Math.Abs(startRotationAngle + 360 - endRotationAngle) < Math.Abs(startRotationAngle - endRotationAngle)) startRotationAngle += 360;
                if (Math.Abs(endRotationAngle + 360 - startRotationAngle) < Math.Abs(endRotationAngle - startRotationAngle)) endRotationAngle += 360;

                return startRotationAngle + (endRotationAngle - startRotationAngle) * MovementController.RotationProgress;
            }
        }

        protected Vector2f CalculatePosition()
        {
            if (!MovementController.IsMoving) return Position;
            else
            {
                return LastPosition + (Position - LastPosition) * (float)MovementController.MovementProgress;
            }

        }

        protected float GetRotationAngleFromDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Up => 180,
                Direction.Down => 0,
                Direction.Left => 90,
                Direction.Right => 270,
                _ => 0
            };
        }

        public virtual void OnDestroy()
        {
            Dispose();
        }

        public override void Dispose()
        {
            UnregisterDestructible();
            base.Dispose();
        }

        public virtual void OnHit()
        {
            SoundManager.Instance.PlayRandomSound("destruction", Position / 64);
            if (IsDestructible && IsAlive) Health--;
            if (Health <= 0) OnDestroy();
        }

        public void RegisterDestructible()
        {
            MessageBus.Instance.PostEvent(MessageType.RegisterDestructible, this, new EventArgs());
        }

        public void UnregisterDestructible()
        {
            MessageBus.Instance.PostEvent(MessageType.UnregisterDestructible, this, new EventArgs());
        }
    }
}
