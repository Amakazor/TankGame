using System;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Core.Gamestate;
using TankGame.Extensions;
using Action = TankGame.Core.Controls.Action;

namespace TankGame.Actors.Pawns.MovementControllers;

public class StandGroundAiMovementController : AiMovementController {
    private const int StandGroundRadius = 3;

    public StandGroundAiMovementController(double delay, Pawn owner) : base(delay, owner, AiMovementControllerType.Stand) {
        TargetPosition = new(-1, -1);
        HomePosition = Owner.Coords;
    }

    [JsonConstructor] public StandGroundAiMovementController(double delay, string controllerType, Action nextAction, double rotationCooldown, double movementCooldown, Vector2i homePosition) : base(delay, AiMovementControllerType.Patrol) {
        AimcType = Enum.Parse<AiMovementControllerType>(controllerType);
        NextAction = nextAction;
        RotationCooldown = rotationCooldown;
        MovementCooldown = movementCooldown;
        HomePosition = homePosition;
    }

    [JsonIgnore] protected Vector2i LastPlayerPosition { get; set; }
    public Vector2i HomePosition { get; set; }

    protected override void DecideOnNextAction() {
        if (CanDoAction() && Owner.CurrentRegion != null) {
            if (CanSeePlayerInUnobstructedLine || (CanSeeActivityInUnobstructedLine && Owner.CurrentRegion != null && Owner.CurrentRegion.HasDestructibleActivity)) { NextAction = Action.Fire; } else {
                if (GamestateManager.Player is null) return;

                Vector2i currentPlayerPosition = GamestateManager.Player.Coords;

                if (LastPlayerPosition != currentPlayerPosition || !GamestateManager.Map.GetFieldFromRegion(TargetPosition)
                                                                                    .IsTraversible()) {
                    LastPlayerPosition = currentPlayerPosition;
                    TargetPosition = GetValidStandGroundPosition();
                    Path = null;
                }

                if (!IsPlayerWithinChaseRadius() && !TargetPosition.Equals(HomePosition)) { NextAction = Action.Nothing; } else {
                    if (!TargetPosition.IsInvalid()) {
                        if (Path != null && Path.Count == 0) Path = null;

                        Path ??= GeneratePath(GamestateManager.Map.GetNodesInRadius(Owner.Coords, SightDistance), new(SightDistance, SightDistance), new Vector2i(SightDistance, SightDistance) + TargetPosition - Owner.Coords);

                        NextAction = Path == null ? Action.Nothing : GetActionFromNextCoords(
                            Path.Pop()
                                .Position + Owner.Coords - new Vector2i(SightDistance, SightDistance)
                        );
                    } else { NextAction = Action.Nothing; }
                }
            }
        } else { NextAction = Action.Nothing; }
    }

    protected bool IsPlayerWithinChaseRadius() {
        Vector2i PlayerCoords = GamestateManager.Player.Coords;
        Vector2i OwnerCoords = Owner.Coords;

        return Math.Abs(PlayerCoords.X - OwnerCoords.X) <= SightDistance && Math.Abs(PlayerCoords.Y - OwnerCoords.Y) <= SightDistance;
    }

    private Vector2i GetValidStandGroundPosition() {
        Vector2i targetPosition = GetClosestValidShootingPositionToPlayer(
            GetValidShootingPositions()
               .FindAll(position => position.ManhattanDistance(HomePosition) <= StandGroundRadius)
        );
        return targetPosition.Equals(new(-1, -1)) ? HomePosition : targetPosition;
    }
}