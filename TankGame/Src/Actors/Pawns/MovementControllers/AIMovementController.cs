using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Actors.Projectiles;
using TankGame.Src.Data;
using TankGame.Src.Data.Map;
using TankGame.Src.Extensions;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal abstract class AIMovementController : MovementController
    {
        protected const int SightDistance = 6;

        public AIMovementController(float delay, Pawn owner) : base(delay, owner)
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
                     return Rotate(GetLineDirectionToPlayer(currentDirection));
                }
            }

            return base.DoAction(currentDirection);
        }

        protected bool CanSeePlayerInUnobstructedLine => IsInLineWithPlayer(Owner.Coords) && IsLineUnobstructed(GamestateManager.Instance.Player.Coords.GetAllVectorsBeetween(Owner.Coords));

        protected bool IsInLineWithPlayer(Vector2i coords)
        {
            Player.Player player = GamestateManager.Instance.Player;
            return player != null ? player.Coords.IsInLine(coords) && player.Coords.ManhattanDistance(coords) <= SightDistance : false;
        }

        protected List<Vector2i> GetAllShootingPositions()
        {
            List<Vector2i> positions = new List<Vector2i>();

            Vector2i playerCoords = GamestateManager.Instance.Player.Coords;
            GameMap gameMap = GamestateManager.Instance.Map;

            for (int x = 0 - SightDistance; x <= SightDistance; x++)
            {
                if (x != 0 && gameMap.GetFieldFromRegion(new Vector2i(playerCoords.X + x, playerCoords.Y)) != null)
                {
                    positions.Add(new Vector2i(playerCoords.X + x, playerCoords.Y));
                }
            }

            for (int y = 0 - SightDistance; y <= SightDistance; y++)
            {
                if (y != 0 && gameMap.GetFieldFromRegion(new Vector2i(playerCoords.X, playerCoords.Y + y)) != null)
                {
                    positions.Add(new Vector2i(playerCoords.X, playerCoords.Y + y));
                }
            }

            return positions;
        }

        protected List<Vector2i> GetValidShootingPositions()
        {
            return GetAllShootingPositions()
                   .OrderBy(position => position.ManhattanDistance(Owner.Coords))
                   .ToList()
                   .FindAll(position => 
                        GamestateManager.Instance.Map.IsFieldTraversible(position) && 
                        IsLineUnobstructed(GamestateManager.Instance.Player.Coords.GetAllVectorsBeetween(position)));
        }

        protected Vector2i GetClosestValidShootingPosition(List<Vector2i> ValidShootingPositions = null)
        {
            ValidShootingPositions ??= GetValidShootingPositions();
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
                    if (field == null || !field.IsTraversible(false, true)) return true;
                    return false;
                });
            }
            else return false;
        }

        protected Direction GetLineDirectionToPlayer(Direction currentDirection)
        {
            if (CanSeePlayerInUnobstructedLine && !(GamestateManager.Instance.Player is null))
            {
                Vector2i playerCoords = GamestateManager.Instance.Player.Coords;

                if      (playerCoords.X > Owner.Coords.X) return Direction.Right;
                else if (playerCoords.X < Owner.Coords.X) return Direction.Left;
                else if (playerCoords.Y > Owner.Coords.Y) return Direction.Down;
                else if (playerCoords.Y < Owner.Coords.Y) return Direction.Up;
            }
            return currentDirection;
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
