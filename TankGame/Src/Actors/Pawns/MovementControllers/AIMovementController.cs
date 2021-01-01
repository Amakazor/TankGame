using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Actors.GameObjects.Activities;
using TankGame.Src.Actors.Projectiles;
using TankGame.Src.Data;
using TankGame.Src.Data.Map;
using TankGame.Src.Extensions;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal abstract class AIMovementController : MovementController
    {
        protected const int SightDistance = 6;
        protected const int PlayerShootingDistance = 5;
        protected const int ActivityShootingDistance = 3;

        public AIMovementController(double delay, Pawn owner) : base(delay, owner)
        {
        }

        public override Direction DoAction(Direction currentDirection)
        {
            if (NextAction == null || CanSeePlayerInUnobstructedLine)
            {
                DecideOnNextAction();
            }

            if (CanSeePlayerInUnobstructedLine && currentDirection != GetLineDirectionToPlayer(currentDirection))
            {
                if (CanDoAction())
                {
                    NextAction = null;
                    return Rotate(currentDirection, GetLineDirectionToPlayer(currentDirection));
                }
            }
            else if (CanSeeActivityInUnobstructedLine && currentDirection != GetLineDirectionToActivity(currentDirection) && Owner.CurrentRegion.HasDestructibleActivity)
            {
                if (CanDoAction())
                {
                    NextAction = null;
                    return Rotate(currentDirection, GetLineDirectionToActivity(currentDirection));
                }
            }

            return base.DoAction(currentDirection);
        }

        protected bool CanSeePlayerInUnobstructedLine => IsInLineWithPlayer(Owner.Coords) && IsLineUnobstructed(GamestateManager.Instance.Player.Coords.GetAllVectorsBeetween(Owner.Coords));
        protected bool CanSeeActivityInUnobstructedLine => IsInLineWithActivity(Owner.Coords) && IsLineUnobstructed(Owner.CurrentRegion.Activity.Coords.GetAllVectorsBeetween(Owner.Coords));

        protected bool IsInLineWithPlayer(Vector2i coords)
        {
            Player.Player player = GamestateManager.Instance.Player;
            return player != null ? player.Coords.IsInLine(coords) && player.Coords.ManhattanDistance(coords) <= PlayerShootingDistance : false;
        }

        protected bool IsInLineWithActivity(Vector2i coords)
        {
            Activity activity = GamestateManager.Instance.Map.GetRegionFromFieldCoords(coords).Activity;
            return activity != null ? activity.Coords.IsInLine(coords) && activity.Coords.ManhattanDistance(coords) <= ActivityShootingDistance : false;
        }

        protected List<Vector2i> GetAllShootingPositions(Vector2i? targetCoords = null, uint? shootingDistance = null)
        {
            List<Vector2i> positions = new List<Vector2i>();

            Vector2i TargetCoords = targetCoords != null ? (Vector2i)targetCoords : GamestateManager.Instance.Player.Coords;
            int ShootingDistance = shootingDistance != null ? (int)shootingDistance : PlayerShootingDistance;
            GameMap gameMap = GamestateManager.Instance.Map;

            for (int x = 0 - ShootingDistance; x <= ShootingDistance; x++)
            {
                if (x != 0 && gameMap.GetFieldFromRegion(new Vector2i(TargetCoords.X + x, TargetCoords.Y)) != null)
                {
                    positions.Add(new Vector2i(TargetCoords.X + x, TargetCoords.Y));
                }
            }

            for (int y = 0 - ShootingDistance; y <= ShootingDistance; y++)
            {
                if (y != 0 && gameMap.GetFieldFromRegion(new Vector2i(TargetCoords.X, TargetCoords.Y + y)) != null)
                {
                    positions.Add(new Vector2i(TargetCoords.X, TargetCoords.Y + y));
                }
            }

            return positions;
        }

        protected List<Vector2i> GetValidShootingPositions(Vector2i? targetCoords = null, uint? shootingDistance = null)
        {
            return GetAllShootingPositions(targetCoords, shootingDistance)
                   .OrderBy(position => position.ManhattanDistance(Owner.Coords))
                   .ToList()
                   .FindAll(position => 
                        GamestateManager.Instance.Map.IsFieldTraversible(position) && 
                        IsLineUnobstructed(GamestateManager.Instance.Player.Coords.GetAllVectorsBeetween(position)));
        }

        protected Vector2i GetClosestValidShootingPositionToPlayer(List<Vector2i> ValidShootingPositions = null)
        {
            ValidShootingPositions ??= GetValidShootingPositions();
            return ValidShootingPositions.Count > 0 ? ValidShootingPositions.First() : new Vector2i(-1, -1);
        }

        protected Vector2i GetClosestValidShootingPositionToActivity()
        {
            List<Vector2i> ValidShootingPositions = GetValidShootingPositions(Owner.CurrentRegion.Activity.Coords, ActivityShootingDistance);
            ValidShootingPositions = ValidShootingPositions.FindAll(position => Owner.CurrentRegion.HasField(position));
            return ValidShootingPositions.Count > 0 ? ValidShootingPositions.First() : new Vector2i(-1, -1);
        }

        protected bool IsLineUnobstructed(List<Vector2i> Line)
        {
            if (Line != null)
            {
                if (Line.Count == 0) return true;

                return !Line.Any(coords =>
                {
                    Field field = GamestateManager.Instance.Map.GetFieldFromRegion(coords);
                    if (field == null || !field.IsShootable(false, true)) return true;
                    return false;
                });
            }
            else return false;
        }

        protected Direction GetLineDirectionToPlayer(Direction currentDirection)
        {
            if (CanSeePlayerInUnobstructedLine && GamestateManager.Instance.Player != null) return GetDirectionFromCoords(currentDirection, GamestateManager.Instance.Player.Coords);
            return currentDirection;
        }

        protected Direction GetLineDirectionToActivity(Direction currentDirection)
        {
            if (CanSeeActivityInUnobstructedLine && Owner.CurrentRegion.Activity != null) return GetDirectionFromCoords(currentDirection, Owner.CurrentRegion.Activity.Coords);
            return currentDirection;
        }

        protected Direction GetDirectionFromCoords(Direction currentDirection, Vector2i targetCoords)
        {

            if (targetCoords.X > Owner.Coords.X) return Direction.Right;
            else if (targetCoords.X < Owner.Coords.X) return Direction.Left;
            else if (targetCoords.Y > Owner.Coords.Y) return Direction.Down;
            else if (targetCoords.Y < Owner.Coords.Y) return Direction.Up;
            else return currentDirection;
        }

        protected Tuple<string, string> GetActionFromNextCoords(Vector2i nextCoords)
        {
            if (nextCoords.X > Owner.Coords.X) return KeyActionType.MoveRight;
            else if (nextCoords.X < Owner.Coords.X) return KeyActionType.MoveLeft;
            else if (nextCoords.Y > Owner.Coords.Y) return KeyActionType.MoveDown;
            else if (nextCoords.Y < Owner.Coords.Y) return KeyActionType.MoveUp;
            else return null;
        }

        protected abstract void DecideOnNextAction();
    }
}
