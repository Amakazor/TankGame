using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Extensions;
using Action = TankGame.Core.Controls.Action;

namespace TankGame.Actors.Pawns.MovementControllers;

public class PatrolAIMovementController : AiMovementController {
    public PatrolAIMovementController(double delay, Pawn owner, List<Vector2i> patrolRoute) : base(delay, owner, AiMovementControllerType.Patrol) {
        PatrolRoute = patrolRoute;
        TargetPosition = new(-1, -1);
    }

    [JsonConstructor] public PatrolAIMovementController(double delay, List<Vector2i> patrolRoute, Stack<Vector2i> currentPatrolRoute, string controllerType, Action nextAction, double rotationCooldown, double movementCooldown) : base(delay, AiMovementControllerType.Patrol) {
        PatrolRoute = patrolRoute;
        CurrentPatrolRoute = currentPatrolRoute;
        AimcType = Enum.Parse<AiMovementControllerType>(controllerType);
        NextAction = nextAction;
        RotationCooldown = rotationCooldown;
        MovementCooldown = movementCooldown;
    }

    public List<Vector2i> PatrolRoute { get; set; }
    public Stack<Vector2i> CurrentPatrolRoute { get; set; }

    protected override void DecideOnNextAction() {
        if (CanDoAction() && Owner.CurrentRegion != null) {
            if (CanSeePlayerInUnobstructedLine || (CanSeeActivityInUnobstructedLine && Owner.CurrentRegion != null && Owner.CurrentRegion.HasDestructibleActivity))
                NextAction = Action.Fire;
            else
                Patrol();
        } else { NextAction = Action.Nothing; }
    }

    protected void Patrol() {
        if (CurrentPatrolRoute is null || !CurrentPatrolRoute.Any()) CurrentPatrolRoute = new(PatrolRoute);

        if (TargetPosition.IsInvalid() || TargetPosition.Equals(Owner.Coords)) TargetPosition = CurrentPatrolRoute.Pop();

        if (!TargetPosition.IsInvalid()) {
            if (Path != null && !Path.Any()) Path = null;

            Path ??= GeneratePath(Owner.CurrentRegion.GetNodesInRegion(), Owner.CurrentRegion.ConvertMapCoordsToRegionFieldCoords(Owner.Coords), Owner.CurrentRegion.ConvertMapCoordsToRegionFieldCoords(TargetPosition));

            NextAction = Path == null ? Action.Nothing : GetActionFromNextCoords(
                Path.Pop()
                    .Position + Owner.CurrentRegion.Coords * Owner.CurrentRegion.FieldsInLine
            );
        } else { NextAction = Action.Nothing; }
    }
}