using SFML.System;
using System;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Projectiles;
using TankGame.Src.Data;
using TankGame.Src.Data.Controls;
using TankGame.Src.Data.Gamestate;
using TankGame.Src.Data.Map;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal abstract class MovementController : ITickable
    {
        protected const double RotationMultiplier = 0.5;

        protected double Delay { get; }
        protected double Cooldown { get; set; }
        protected Tuple<string, string> NextAction { get; set; }
        protected Pawn Owner { get; }
        public bool IsRotating { get; protected set; }
        public bool IsMoving { get; protected set; }
        public double RotationCooldown { get; protected set; }
        public double MovementCooldown { get; protected set; }
        public double RotationProgress => 1 - (Cooldown / RotationCooldown);
        public double MovementProgress => 1 - (Cooldown / MovementCooldown);

        public MovementController(double delay, Pawn owner)
        {
            Delay = delay;
            Owner = owner;
        }

        public virtual Direction DoAction(Direction currentDirection)
        {
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

            return nextDirection == currentDirection ? Move(currentDirection, nextDirection) : Rotate(currentDirection, nextDirection);
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

                    if (nextField.GameObject == null || nextField.GameObject.IsTraversible)
                    {
                        IsMoving = true;
                        float averageTraversabilityMultiplier = (nextField.TraversabilityMultiplier + prevField.TraversabilityMultiplier) / 2;
                        MovementCooldown = averageTraversabilityMultiplier * Delay * GamestateManager.Instance.WeatherModifier;
                        SetCooldown(averageTraversabilityMultiplier);

                        if (nextField.GameObject != null)
                        {
                            if (nextField.GameObject.DestructabilityData.DestroyOnEntry)
                            {
                                nextField.GameObject.OnDestroy();
                            }
                        }
                    }
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

        public Direction Rotate(Direction currentDirection, Direction direction)
        {
            IsRotating = true;

            double newRotationMultiplier = RotationMultiplier * (IsDirectionOposite(currentDirection, direction) ? 2 : 1);

            RotationCooldown = newRotationMultiplier * Delay * GamestateManager.Instance.WeatherModifier;
            SetCooldown(newRotationMultiplier);

            return direction;
        }

        public virtual bool CanDoAction()
        {
            bool canDoAction = Cooldown == 0 && Owner.IsAlive;
            if (canDoAction) ClearStatus();
            return canDoAction;
        }

        public void ClearStatus()
        {
            IsRotating = false;
            IsMoving = false;
            RotationCooldown = 0;
            MovementCooldown = 0;
            Owner.LastDirection = Owner.Direction;
            Owner.LastPosition = Owner.Position;
        }

        public void ClearAction() => NextAction = null;
        protected void SetRandomizedCooldown() => SetCooldown(GamestateManager.Instance.Random.NextDouble() / 4);
        protected void SetCooldown(double multiplier = 1) => Cooldown = Delay * multiplier * GamestateManager.Instance.WeatherModifier;

        public void Tick(float deltaTime)
        {
            if (Cooldown > 0) Cooldown -= deltaTime;
            if (Cooldown < 0) Cooldown = 0;

            GameMap gameMap = GamestateManager.Instance.Map;

            if (IsMoving && gameMap.GetFieldFromRegion(Owner.Coords) != null)
            {
                gameMap.GetFieldFromRegion(Owner.Coords).PawnOnField = Owner;
            }

            if (IsMoving && MovementProgress >= 0.8 && gameMap.GetFieldFromRegion(Owner.LastCoords)?.PawnOnField == Owner)
            {
                gameMap.GetFieldFromRegion(Owner.LastCoords).PawnOnField = null;
            }
        }

        public bool IsDirectionOposite(Direction lastDirection, Direction currentDirection) => (lastDirection == Direction.Up && currentDirection == Direction.Down) || (lastDirection == Direction.Down && currentDirection == Direction.Up) || (lastDirection == Direction.Left && currentDirection == Direction.Right) || (lastDirection == Direction.Right && currentDirection == Direction.Left);
        

        public void RegisterTickable()
        {
            
        }

        public void UnregisterTickable()
        {
            
        }
    }
}