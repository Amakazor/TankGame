using System;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Core.Gamestate;
using TankGame.Extensions;
using Action = TankGame.Core.Controls.Action;

namespace TankGame.Actors.Pawns.MovementControllers;

public class ChaseAIMovementController : AiMovementController {
    public ChaseAIMovementController(double delay, Pawn owner, AiMovementControllerType type = AiMovementControllerType.Chase) : base(delay, owner, type)
        => TargetPosition = new(-1, -1);

    [JsonConstructor] public ChaseAIMovementController(double delay, string controllerType, Action nextAction, double rotationCooldown, double movementCooldown) : base(delay, AiMovementControllerType.Patrol) {
        AimcType = Enum.Parse<AiMovementControllerType>(controllerType);
        NextAction = nextAction;
        RotationCooldown = rotationCooldown;
        MovementCooldown = movementCooldown;
    }

    [JsonIgnore] protected Vector2i LastPlayerPosition { get; set; }

    protected override void DecideOnNextAction() {
        if (CanDoAction()) {
            if (CanSeePlayerInUnobstructedLine || (CanSeeActivityInUnobstructedLine && Owner.CurrentRegion != null && Owner.CurrentRegion.HasDestructibleActivity)) { NextAction = Action.Fire; } else {
                if (Owner.CurrentRegion != null && Owner.CurrentRegion.HasDestructibleActivity)
                    ChaseActivity();
                else
                    ChasePlayer();
            }
        } else { NextAction = Action.Nothing; }
    }

    protected bool IsPlayerWithinChaseRadius() {
        Vector2i PlayerCoords = GamestateManager.Player.Coords;
        Vector2i OwnerCoords = Owner.Coords;

        return Math.Abs(PlayerCoords.X - OwnerCoords.X) <= SightDistance && Math.Abs(PlayerCoords.Y - OwnerCoords.Y) <= SightDistance;
    }

    protected void ChasePlayer() {
        if (Owner.CurrentRegion != null) {
            if (!IsPlayerWithinChaseRadius()) { NextAction = Action.Nothing; } else {
                Vector2i currentPlayerPosition = GamestateManager.Player.Coords;

                if (LastPlayerPosition != currentPlayerPosition || !GamestateManager.Map.GetFieldFromRegion(TargetPosition)
                                                                                    .IsTraversible()) {
                    LastPlayerPosition = currentPlayerPosition;
                    TargetPosition = GetClosestValidShootingPositionToPlayer();
                    Path = null;
                }

                if (TargetPosition.IsValid()) {
                    if (Path is { Count: 0 }) Path = null;

                    Path ??= GeneratePath(GamestateManager.Map.GetNodesInRadius(Owner.Coords, SightDistance), new(SightDistance, SightDistance), new Vector2i(SightDistance, SightDistance) + TargetPosition - Owner.Coords);

                    NextAction = Path == null ? Action.Nothing : GetActionFromNextCoords(
                        Path.Pop()
                            .Position + Owner.Coords - new Vector2i(SightDistance, SightDistance)
                    );
                } else { NextAction = Action.Nothing; }
            }
        } else { NextAction = Action.Nothing; }
    }

    protected void ChaseActivity() {
        if (Owner.CurrentRegion == null) return;
        if (Path is { Count: 0 }) Path = null;

        Path ??= GeneratePath(Owner.CurrentRegion.GetNodesInRegion(), Owner.CurrentRegion.ConvertMapCoordsToRegionFieldCoords(Owner.Coords), Owner.CurrentRegion.ConvertMapCoordsToRegionFieldCoords(GetClosestValidShootingPositionToActivity()));

        NextAction = Path == null ? Action.Nothing : GetActionFromNextCoords(
            Path.Pop()
                .Position + Owner.CurrentRegion.Coords * Owner.CurrentRegion.FieldsInLine
        );
    }
}