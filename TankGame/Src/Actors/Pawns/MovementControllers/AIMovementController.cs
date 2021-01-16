using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Actors.GameObjects.Activities;
using TankGame.Src.Data.Controls;
using TankGame.Src.Data.Gamestate;
using TankGame.Src.Data.Map;
using TankGame.Src.Extensions;
using TankGame.Src.Pathfinding;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal abstract class AIMovementController : MovementController
    {
        public string AimcType { get; }

        protected const int BaseSightDistance = 6;
        protected const int BasePlayerShootinhDistance = 5;
        protected const int BaseActivitityShootingDistance = 3;
        protected int SightDistance => (int)Math.Floor(BaseSightDistance * (1 / GamestateManager.Instance.WeatherModifier));
        protected int PlayerShootingDistance => (int)Math.Floor(BasePlayerShootinhDistance * (1 / GamestateManager.Instance.WeatherModifier));
        protected int ActivityShootingDistance => (int)Math.Floor(BaseActivitityShootingDistance * (1 / GamestateManager.Instance.WeatherModifier));

        protected AIMovementController(double delay, Pawn owner, string aimcType) : base(delay, owner)
        {
            AimcType = aimcType;
            SetCooldown(0.5);
        }

        public XmlNode SerializeAIMovementControllerType(XmlDocument xmlDocument)
        {
            XmlElement typeElement = xmlDocument.CreateElement("aimc");
            typeElement.InnerText = AimcType;
            return typeElement;
        }

        public override Direction DoAction(Direction currentDirection)
        {
            if (NextAction == null || CanSeePlayerInUnobstructedLine || CanSeeActivityInUnobstructedLine)
            {
                DecideOnNextAction();
            }

            if (CanDoAction() && (CanSeePlayerInUnobstructedLine || CanSeeActivityInUnobstructedLine))
            {
                if (CanSeePlayerInUnobstructedLine && currentDirection != GetLineDirectionToPlayer(currentDirection))
                {
                    NextAction = null;
                    return Rotate(currentDirection, GetLineDirectionToPlayer(currentDirection));
                }
                else if (!CanSeePlayerInUnobstructedLine && CanSeeActivityInUnobstructedLine && currentDirection != GetLineDirectionToActivity(currentDirection) && Owner.CurrentRegion.HasDestructibleActivity)
                {
                    NextAction = null;
                    return Rotate(currentDirection, GetLineDirectionToActivity(currentDirection));
                }
            }

            return base.DoAction(currentDirection);
        }

        protected bool CanSeePlayerInUnobstructedLine => IsInLineWithPlayer(Owner.Coords) && IsLineUnobstructed(GamestateManager.Instance.Player.Coords.GetAllVectorsBeetween(Owner.Coords));
        protected bool CanSeeActivityInUnobstructedLine => IsInLineWithActivity(Owner.Coords) && IsLineUnobstructed(Owner.CurrentRegion.Activity.Coords.GetAllVectorsBeetween(Owner.Coords));
        protected Vector2i TargetPosition { get; set; }
        protected Stack<Node> Path { get; set; }

        protected bool IsInLineWithPlayer(Vector2i coords)
        {
            Player.Player player = GamestateManager.Instance.Player;
            return player != null && player.Coords.IsInLine(coords) && player.Coords.ManhattanDistance(coords) <= PlayerShootingDistance;
        }

        protected bool IsInLineWithActivity(Vector2i coords)
        {
            Activity activity = GamestateManager.Instance.Map.GetRegionFromFieldCoords(coords)?.Activity;
            return activity != null && activity.Coords.IsInLine(coords) && activity.Coords.ManhattanDistance(coords) <= ActivityShootingDistance;
        }

        protected List<Vector2i> GetAllShootingPositions(Vector2i targetCoords, uint? shootingDistance = null)
        {
            List<Vector2i> positions = new List<Vector2i>();

            int ShootingDistance = shootingDistance != null ? (int)shootingDistance : PlayerShootingDistance;
            GameMap gameMap = GamestateManager.Instance.Map;

            for (int x = 0 - ShootingDistance; x <= ShootingDistance; x++)
            {
                if (x != 0 && gameMap.GetFieldFromRegion(new Vector2i(targetCoords.X + x, targetCoords.Y)) != null)
                {
                    positions.Add(new Vector2i(targetCoords.X + x, targetCoords.Y));
                }
            }

            for (int y = 0 - ShootingDistance; y <= ShootingDistance; y++)
            {
                if (y != 0 && gameMap.GetFieldFromRegion(new Vector2i(targetCoords.X, targetCoords.Y + y)) != null)
                {
                    positions.Add(new Vector2i(targetCoords.X, targetCoords.Y + y));
                }
            }

            return positions;
        }

        protected List<Vector2i> GetValidShootingPositions(Vector2i? targetCoords = null, uint? shootingDistance = null)
        {
            Vector2i TargetCoords = targetCoords != null ? (Vector2i)targetCoords : GamestateManager.Instance.Player.Coords;

            return GetAllShootingPositions(TargetCoords, shootingDistance)
                   .OrderBy(position => position.ManhattanDistance(Owner.Coords))
                   .ThenBy(postion => GamestateManager.Instance.Map.GetFieldFromRegion(postion).TraversabilityMultiplier)
                   .ToList()
                   .FindAll(position => 
                        GamestateManager.Instance.Map.IsFieldTraversible(position) && 
                        IsLineUnobstructed(TargetCoords.GetAllVectorsBeetween(position)));
        }

        protected Vector2i GetClosestValidShootingPositionToPlayer(List<Vector2i> ValidShootingPositions = null)
        {
            ValidShootingPositions ??= GetValidShootingPositions();
            return ValidShootingPositions.Count > 0 ? ValidShootingPositions.First() : new Vector2i(-1, -1);
        }

        protected Vector2i GetClosestValidShootingPositionToActivity()
        {
            List<Vector2i> ValidShootingPositions = GetValidShootingPositions(Owner.CurrentRegion.Activity.Coords, (uint)ActivityShootingDistance);
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

        protected Stack<Node> GeneratePath(List<List<Node>> grid, Vector2i start, Vector2i end)
        {
            return new AStar(grid).FindPath(start, end);
        }

        protected abstract void DecideOnNextAction();
    }
}
