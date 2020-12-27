using SFML.System;
using System;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Projectiles;
using TankGame.Src.Data;
using TankGame.Src.Data.Map;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal abstract class MovementController : ITickable
    {
        protected double Delay { get; }
        protected double Cooldown { get; set; }
        protected Tuple<string, string> NextAction { get; set; }
        protected Pawn Owner { get; }

        public MovementController(double delay, Pawn owner)
        {
            Delay = delay;
            Owner = owner;
        }

        public virtual Direction DoAction(Direction currentDirection)
        {
            Console.WriteLine("Trying to do action as " + Owner.GetType().ToString());

            if (CanDoAction() && NextAction != null) return NextAction.Equals(KeyActionType.Shoot) ? Shoot(currentDirection) : MoveOrRotate(currentDirection);
            else if (NextAction is null && Owner is Enemy) SetRandomizedCooldown();
            return currentDirection;
        }

        public Direction MoveOrRotate(Direction currentDirection)
        {
            Direction nextDirection;

            if (NextAction.Equals(KeyActionType.MoveDown)) nextDirection = Direction.Down;
            else if (NextAction.Equals(KeyActionType.MoveUp)) nextDirection = Direction.Up;
            else if (NextAction.Equals(KeyActionType.MoveLeft)) nextDirection = Direction.Left;
            else if (NextAction.Equals(KeyActionType.MoveRight)) nextDirection = Direction.Right;
            else return currentDirection;

            return nextDirection == currentDirection ? Move(currentDirection, nextDirection) : Rotate(nextDirection);
        }

        public Direction Move(Direction currentDirection, Direction nextDirection)
        {
            Vector2i nextCoords = GetNextCoordsFromDirection(nextDirection, Owner.Coords);
            ClearAction();
            if (!nextCoords.Equals(new Vector2i(-1, -1)))
            {
                GameMap gameMap = GamestateManager.Instance.Map;
                Field nextField = gameMap.GetFieldFromRegion(nextCoords);
                Field prevField = gameMap.GetFieldFromRegion(Owner.Coords);

                if (nextField != null & prevField != null && nextField.IsTraversible())
                {
                    Owner.Coords = nextCoords;
                    nextField.PawnOnField = Owner;
                    prevField.PawnOnField = null;

                    if (nextField.GameObject != null && nextField.GameObject.TraversibilityData.IsTraversible)
                    {
                        SetCooldown(nextField.TraversabilityData.SpeedModifier, nextField.GameObject.TraversibilityData.SpeedModifier);

                        if (nextField.GameObject.DestructabilityData.DestroyOnEntry)
                        {
                            nextField.GameObject.Dispose();
                            nextField.GameObject = null;
                        }
                    }
                    else SetCooldown(nextField.TraversabilityData.SpeedModifier);
                }
                else SetRandomizedCooldown();
                return nextDirection;
            }
            else return currentDirection;
        }

        protected Vector2i GetNextCoordsFromDirection(Direction nextDirection, Vector2i currentCoords)
        {
            return nextDirection switch
            {
                Direction.Down => currentCoords + new Vector2i(0, 1),
                Direction.Up => currentCoords + new Vector2i(0, -1),
                Direction.Left => currentCoords + new Vector2i(-1, 0),
                Direction.Right => currentCoords + new Vector2i(1, 0),
                _ => new Vector2i(-1, -1),
            };
        }

        public Direction Shoot(Direction direction)
        {
            SetCooldown();
            ClearAction();
            new Projectile(Owner.Position, direction, Owner);
            return direction;
        }

        public Direction Rotate(Direction direction)
        {
            SetCooldown(0.25F);
            return direction;
        }

        public virtual bool CanDoAction() => Cooldown == 0 && Owner.IsAlive;

        public void ClearAction() => NextAction = null;

        protected void SetRandomizedCooldown() => SetCooldown(GamestateManager.Instance.Random.NextDouble() / 4);

        protected void SetCooldown(double multiplier = 1, double multiplier2 = 1)
        {
            Cooldown = Delay * multiplier * multiplier2;
        }

        public void Tick(float deltaTime)
        {
            if (Cooldown > 0) Cooldown -= deltaTime;
            if (Cooldown < 0) Cooldown = 0;
        }

        public void RegisterTickable()
        {
            
        }

        public void UnregisterTickable()
        {
            
        }
    }
}