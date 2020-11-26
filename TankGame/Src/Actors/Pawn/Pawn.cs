using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Actors.Pawn.MovementControllers;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Pawn
{
    internal abstract class Pawn : TickableActor, IDestructible
    {
        public Direction Direction { get; protected set; }
        public int HP { get; protected set; }
        public MovementController MovementController { get; protected set; }
        private Texture Texture { get; }
        private SpriteComponent Surface { get; }
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
            Surface = new SpriteComponent(Position, Size, this, Texture, new Color(255, 255, 255, 255));
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { Surface };
        }

        public override void Tick(float deltaTime)
        {
            if (MovementController != null && MovementController.CanDoAction())
            {
                MovementController.DoAction(Direction);
            }
        }

        public abstract bool IsAlive();
        public abstract void OnDestroy(Actor other);
        public abstract void OnHit(Actor other);
    }
}
