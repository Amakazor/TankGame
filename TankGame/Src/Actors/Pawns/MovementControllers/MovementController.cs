using SFML.System;
using System;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Actors.Projectiles;
using TankGame.Src.Data;
using TankGame.Src.Events;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal abstract class MovementController : ITickable
    {
        protected float Delay { get; }
        protected float Cooldown { get; set; }
        protected Tuple<string, string> NextAction { get; set; }
        protected Pawn Owner { get; }

        public MovementController(float delay, Pawn owner)
        {
            Delay = delay;
            Owner = owner;
        }

        public virtual Direction DoAction(Direction currentDirection)
        {
            Console.WriteLine("Trying to do action as " + Owner.GetType().ToString());

            if (CanDoAction())
            {
                Direction newDirection = currentDirection;

                if (NextAction.Equals(KeyActionType.Shoot))
                {
                    Shoot(currentDirection);
                }
                else
                {
                    newDirection = Move(currentDirection);
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

            Console.WriteLine("Trying to move " + Owner.GetType().ToString());

            if (NextAction.Equals(KeyActionType.MoveDown))
            {
                nextCoords = currentCoords + new Vector2i(0, 1);
                nextDirection = Direction.Down;
            }
            else if (NextAction.Equals(KeyActionType.MoveUp))
            {
                nextCoords = currentCoords + new Vector2i(0, -1);
                nextDirection = Direction.Up;
            }
            else if (NextAction.Equals(KeyActionType.MoveLeft))
            {
                nextCoords = currentCoords + new Vector2i(-1, 0);
                nextDirection = Direction.Left;
            }
            else if (NextAction.Equals(KeyActionType.MoveRight))
            {
                nextCoords = currentCoords + new Vector2i(1, 0);
                nextDirection = Direction.Right;
            }
            else return currentDirection;

            Console.WriteLine("\tCan move, valid action");

            if (nextDirection == currentDirection)
            {
                Console.WriteLine("\tCorrect direction, moving");

                if (nextCoords.X > -1 && nextCoords.Y > -1)
                {
                    Console.WriteLine("\tCan move, valid coords");
                    Field nextField = GamestateManager.Instance.GetMap().GetFieldFromRegion(nextCoords);
                    Field prevField = GamestateManager.Instance.GetMap().GetFieldFromRegion(currentCoords);

                    if (nextField.IsTraversible())
                    {
                        Console.WriteLine("\tCan move, field traversible");
                        Console.WriteLine("\tMoving");

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
                        else
                        {
                            SetCooldown(nextField.TraversabilityData.SpeedModifier);
                        }

                        Console.WriteLine("\tOwner " + Owner.GetType().ToString() + " was moved to: " + Owner.Coords.X + " " + Owner.Coords.Y);
                    }
                }
                ClearAction();
            }
            else
            {
                Console.WriteLine("\tIncorrect direction, rotating");
                if (Cooldown == 0)
                {
                    Cooldown = Delay / 4;
                }
            }

            return nextDirection;
        }

        public virtual void Shoot(Direction direction)
        {
            if (Cooldown == 0)
            {
                Cooldown = Delay;
            }

            ClearAction();
        }

        public virtual bool CanDoAction()
        {
            return Cooldown == 0 && Owner.IsAlive();
        }

        public void ClearAction()
        {
            NextAction = null;
        }

        protected void SetCooldown(float multiplier)
        {
            Cooldown = Delay * multiplier;
        }
        
        protected void SetCooldown(float multiplier, float multiplier2)
        {
            Cooldown = Delay * multiplier * multiplier2;
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
            
        }

        public void UnregisterTickable()
        {
            
        }
    }
}