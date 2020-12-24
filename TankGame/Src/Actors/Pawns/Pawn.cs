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
        public int HP { get; set; }
        public MovementController MovementController { get; set; }
        private Texture Texture { get; }
        private SpriteComponent PawnSprite { get; }
        public Vector2i Coords
        {
            get
            {
                return new Vector2i((int)(Position.X / Size.X), (int)(Position.Y / Size.Y));
            }

            set
            {
                Position = new Vector2f(value.X * Size.X, value.Y * Size.Y);
            }
        }

        public Pawn(Vector2f position, Vector2f size, Texture texture) : base(position, size)
        {
            Texture = texture;
            PawnSprite = new SpriteComponent(Position, Size, this, Texture, new Color(255, 255, 255, 255));
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
                else
                {
                    if (MovementController is PlayerMovementController)
                    {
                        MovementController.ClearAction();
                    }
                }

                MovementController.Tick(deltaTime);
            }
        }

        protected virtual void UpdatePosition()
        {
            PawnSprite.SetPosition(Position);
            PawnSprite.SetDirection(Direction);
        }

        public bool IsAlive()
        {
            return HP > 0;
        }

        public void OnDestroy(Actor other)
        {
            Dispose();
        }

        public void OnHit(Actor other)
        {
            throw new NotImplementedException();
        }

        public int GetHealth()
        {
            return HP;
        }

        public void SetHealth(int amount)
        {
            HP = amount;
        }

        public bool IsDestructible()
        {
            return true;
        }
    }
}
