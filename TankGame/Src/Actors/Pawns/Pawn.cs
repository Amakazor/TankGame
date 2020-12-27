using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Actors.Pawns.MovementControllers;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Pawns
{
    internal abstract class Pawn : TickableActor, IDestructible
    {
        public Direction Direction { get; protected set; }
        public int Health { get; set; }
        public MovementController MovementController { get; set; }
        private Texture Texture { get; }
        private SpriteComponent PawnSprite { get; }
        public Vector2i Coords
        {
            get => new Vector2i((int)(Position.X / Size.X), (int)(Position.Y / Size.Y));
            set => Position = new Vector2f(value.X * Size.X, value.Y * Size.Y);
        }
        public bool IsAlive => Health > 0;
        public bool IsDestructible => true;

        public Pawn(Vector2f position, Vector2f size, Texture texture, int health) : base(position, size)
        {
            Health = health;
            PawnSprite = new SpriteComponent(Position, Size, this, Texture = texture, new Color(255, 255, 255, 255));
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { PawnSprite };
        }

        public override void Tick(float deltaTime)
        {
            if (MovementController != null)
            {
                if (MovementController.CanDoAction())
                {
                    Direction = MovementController.DoAction(Direction);
                    UpdatePosition();
                }
                else if (MovementController is PlayerMovementController) MovementController.ClearAction();

                MovementController.Tick(deltaTime);
            }
        }

        protected virtual void UpdatePosition()
        {
            PawnSprite.SetPosition(Position);
            PawnSprite.SetDirection(Direction);
        }

        public void OnDestroy(Actor other)
        {
            Dispose();
        }

        public void OnHit(Actor other)
        {
            throw new NotImplementedException();
        }
    }
}
