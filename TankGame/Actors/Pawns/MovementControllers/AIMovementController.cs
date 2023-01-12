using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Actors.GameObjects.Activities;
using TankGame.Core.Gamestate;
using TankGame.Core.Map;
using TankGame.Extensions;
using TankGame.Pathfinding;
using Action = TankGame.Core.Controls.Action;

namespace TankGame.Actors.Pawns.MovementControllers;

[JsonDerivedType(typeof(PatrolAIMovementController), "Patrol"), JsonDerivedType(typeof(ChaseAIMovementController), "Chase"), JsonDerivedType(typeof(RandomAiMovementController), "Random"), JsonDerivedType(typeof(StandGroundAiMovementController), "Stand")]
public abstract class AiMovementController : MovementController {
    private const int BaseSightDistance = 6;
    private const int BasePlayerShootingDistance = 5;
    private const int BaseActivityShootingDistance = 3;

    protected AiMovementController(double delay, Pawn owner, AiMovementControllerType aimcType) : base(delay, owner) {
        AimcType = aimcType;
        SetCooldown(0.5);
    }

    protected AiMovementController(double delay, AiMovementControllerType aimcType) : base(delay) {
        AimcType = aimcType;
        SetCooldown(0.5);
    }

    [JsonIgnore] public AiMovementControllerType AimcType { get; set; }
    public string ControllerType => AimcType.ToString();

    [JsonIgnore] protected static int SightDistance => (int)Math.Floor(BaseSightDistance * (1 / GamestateManager.WeatherModifier));

    [JsonIgnore] protected static int PlayerShootingDistance => (int)Math.Floor(BasePlayerShootingDistance * (1 / GamestateManager.WeatherModifier));

    [JsonIgnore] protected static int ActivityShootingDistance => (int)Math.Floor(BaseActivityShootingDistance * (1 / GamestateManager.WeatherModifier));

    protected bool CanSeePlayerInUnobstructedLine => IsInLineWithPlayer(Owner.Coords) && IsLineUnobstructed(GamestateManager.Player.Coords.GetAllVectorsBeetween(Owner.Coords));

    protected bool CanSeeActivityInUnobstructedLine => IsInLineWithActivity(Owner.Coords) && IsLineUnobstructed(Owner.CurrentRegion.Activity.Coords.GetAllVectorsBeetween(Owner.Coords));

    protected Vector2i TargetPosition { get; set; }
    protected Stack<Node> Path { get; set; }

    public override Direction DoAction(Direction currentDirection) {
        if (NextAction == Action.Nothing || CanSeePlayerInUnobstructedLine || CanSeeActivityInUnobstructedLine) DecideOnNextAction();

        if (CanDoAction() && (CanSeePlayerInUnobstructedLine || CanSeeActivityInUnobstructedLine)) {
            if (CanSeePlayerInUnobstructedLine && currentDirection != GetLineDirectionToPlayer(currentDirection)) {
                NextAction = Action.Nothing;
                return Rotate(currentDirection, GetLineDirectionToPlayer(currentDirection));
            }

            if (!CanSeePlayerInUnobstructedLine && CanSeeActivityInUnobstructedLine && currentDirection != GetLineDirectionToActivity(currentDirection) && Owner.CurrentRegion.HasDestructibleActivity) {
                NextAction = Action.Nothing;
                return Rotate(currentDirection, GetLineDirectionToActivity(currentDirection));
            }
        }

        return base.DoAction(currentDirection);
    }

    protected static bool IsInLineWithPlayer(Vector2i coords) {
        Player.Player player = GamestateManager.Player;
        return player != null && player.Coords.IsInLine(coords) && player.Coords.ManhattanDistance(coords) <= PlayerShootingDistance;
    }

    protected static bool IsInLineWithActivity(Vector2i coords) {
        Activity activity = GamestateManager.Map.GetRegionFromFieldCoords(coords)
                                           ?.Activity;
        return activity != null && activity.Coords.IsInLine(coords) && activity.Coords.ManhattanDistance(coords) <= ActivityShootingDistance;
    }

    protected IEnumerable<Vector2i> GetAllShootingPositions(Vector2i targetCoords, int shootingDistance = -1) {
        if (shootingDistance == -1) shootingDistance = PlayerShootingDistance;

        List<Vector2i> positions = new();
        GameMap gameMap = GamestateManager.Map;

        for (int x = 0 - shootingDistance; x <= shootingDistance; x++)
            if (x != 0 && gameMap.GetFieldFromRegion(new(targetCoords.X + x, targetCoords.Y)) != null)
                positions.Add(new(targetCoords.X + x, targetCoords.Y));

        for (int y = 0 - shootingDistance; y <= shootingDistance; y++)
            if (y != 0 && gameMap.GetFieldFromRegion(new(targetCoords.X, targetCoords.Y + y)) != null)
                positions.Add(new(targetCoords.X, targetCoords.Y + y));

        return positions;
    }

    protected List<Vector2i> GetValidShootingPositions(Vector2i? targetCoords = null, int shootingDistance = -1) {
        targetCoords ??= GamestateManager.Player.Coords;

        return GetAllShootingPositions((Vector2i)targetCoords, shootingDistance)
              .OrderBy(position => position.ManhattanDistance(Owner.Coords))
              .ThenBy(position => GamestateManager.Map.GetFieldFromRegion(position).TraversabilityMultiplier)
              .ToList()
              .FindAll(position => GamestateManager.Map.IsFieldTraversible(position) && IsLineUnobstructed(targetCoords?.GetAllVectorsBeetween(position)));
    }

    protected Vector2i GetClosestValidShootingPositionToPlayer(List<Vector2i>? validShootingPositions = null) {
        validShootingPositions ??= GetValidShootingPositions();
        return validShootingPositions.Count > 0 ? validShootingPositions.First() : new(-1, -1);
    }

    protected Vector2i GetClosestValidShootingPositionToActivity() {
        var validShootingPositions = GetValidShootingPositions(Owner.CurrentRegion.Activity.Coords, ActivityShootingDistance)
           .FindAll(position => Owner.CurrentRegion.HasField(position));
        return validShootingPositions.Count > 0 ? validShootingPositions.First() : new(-1, -1);
    }

    protected static bool IsLineUnobstructed(List<Vector2i> line) {
        if (line       == null) return false;
        if (line.Count == 0) return true;

        return !line.Any(
            coords => !GamestateManager.Map.GetFieldFromRegion(coords)
                                      ?.IsShootable(false, true) ?? false
        );
    }

    protected Direction GetLineDirectionToPlayer(Direction currentDirection)
        => CanSeePlayerInUnobstructedLine && GamestateManager.Player != null ? GetDirectionFromCoords(currentDirection, GamestateManager.Player.Coords) : currentDirection;

    protected Direction GetLineDirectionToActivity(Direction currentDirection)
        => CanSeeActivityInUnobstructedLine ? GetDirectionFromCoords(currentDirection, Owner.CurrentRegion.Activity.Coords) : currentDirection;

    protected Direction GetDirectionFromCoords(Direction currentDirection, Vector2i targetCoords) {
        if (targetCoords.X > Owner.Coords.X) return Direction.Right;
        if (targetCoords.X < Owner.Coords.X) return Direction.Left;
        if (targetCoords.Y > Owner.Coords.Y) return Direction.Down;
        if (targetCoords.Y < Owner.Coords.Y) return Direction.Up;
        return currentDirection;
    }

    protected Action GetActionFromNextCoords(Vector2i nextCoords) {
        if (nextCoords.X > Owner.Coords.X) return Action.MoveRight;
        if (nextCoords.X < Owner.Coords.X) return Action.MoveLeft;
        if (nextCoords.Y > Owner.Coords.Y) return Action.MoveDown;
        if (nextCoords.Y < Owner.Coords.Y) return Action.MoveUp;
        return Action.Nothing;
    }

    protected static Stack<Node> GeneratePath(List<List<Node>> grid, Vector2i start, Vector2i end)
        => new AStar(grid).FindPath(start, end);

    protected abstract void DecideOnNextAction();
}