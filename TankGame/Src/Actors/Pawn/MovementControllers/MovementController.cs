using SFML.System;
using System;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Data;
using TankGame.Src.Events;

namespace TankGame.Src.Actors.Pawn.MovementControllers
{
    internal abstract class MovementController : ITickable
    {
        private float Delay { get; }
        private float Cooldown { get; set; }
        protected Tuple<string, string> NextAction { get; set; }
        private Pawn Owner { get; }

        public MovementController(float delay, Pawn owner)
        {
            Delay = delay;
            Owner = owner;
        }

        public virtual Direction DoAction(Direction currentDirection)
        {
            if (CanDoAction())
            {
                Direction newDirection = currentDirection;

                if (NextAction == KeyActionType.Shoot)
                {
                    Shoot(currentDirection);
                }
                else
                {
                    newDirection = Move(currentDirection);
                }

                NextAction = null;

                if (Cooldown == 0)
                {
                    Cooldown = Delay;
                }

                return newDirection;
            }
            else return currentDirection;
        }

        public Direction Move(Direction currentDirection)
        {
            Vector2i currentCoords = Owner.Coords;
            Vector2i nextCoords;

            Direction nextDirection;

            if (NextAction == KeyActionType.MoveDown)
            {
                nextCoords = currentCoords + new Vector2i(0, 1);
                nextDirection = Direction.Down;
            }
            else if (NextAction == KeyActionType.MoveUp)
            {
                nextCoords = currentCoords + new Vector2i(0, -1);
                nextDirection = Direction.Up;
            }
            else if (NextAction == KeyActionType.MoveLeft)
            {
                nextCoords = currentCoords + new Vector2i(-1, 0);
                nextDirection = Direction.Left;
            }
            else if (NextAction == KeyActionType.MoveRight)
            {
                nextCoords = currentCoords + new Vector2i(1, 0);
                nextDirection = Direction.Right;
            }
            else return currentDirection;

            if (nextCoords.X > -1 && nextCoords.Y > -1)
            {
                FieldData fieldData = GamestateManager.Instance.GetMap().GetFieldDataFromRegion(nextCoords);

                if (fieldData.IsTraversible)
                {
                    Owner.Coords = nextCoords;
                    SetCooldown(fieldData.SpeedModifier);
                }
            }
            return nextDirection;
        }

        public void Shoot(Direction direction)
        {
            throw new NotImplementedException();
        }

        public virtual bool CanDoAction()
        {
            return Cooldown == 0 && NextAction != null && Owner.IsAlive();
        }

        protected void SetCooldown(float multiplier)
        {
            Cooldown = Delay * multiplier;
        }

        public void Tick(float deltaTime)
        {
            if (Cooldown > 0)
            {
                Cooldown -= deltaTime;
            }

            if (Cooldown < 0)
            {
                Cooldown = 0;
            }
        }

        public void RegisterTickable()
        {
            MessageBus.Instance.PostEvent(MessageType.RegisterTickable, this, new EventArgs());
        }

        public void UnregisterTickable()
        {
            MessageBus.Instance.PostEvent(MessageType.UnregisterTickable, this, new EventArgs());
        }
    }
}