using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Extensions;
using Action = TankGame.Core.Controls.Action;

namespace TankGame.Actors.Pawns.MovementControllers;

public class RandomAiMovementController : AiMovementController {
    public RandomAiMovementController(double delay, Pawn owner) : base(delay, owner, AiMovementControllerType.Random)
        => TargetPosition = new(-1, -1);

    [JsonConstructor] public RandomAiMovementController(double delay, List<Vector2i> patrolRoute, Stack<Vector2i> currentPatrolRoute, string controllerType, Action nextAction, double rotationCooldown, double movementCooldown) : base(delay, AiMovementControllerType.Patrol) {
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
            PatrolRoute ??= Owner.CurrentRegion.GetNodesInRegion()
                                 .SelectMany(node => node)
                                 .ToList()
                                 .FindAll(node => node.Walkable)
                                 .Select(node => node.Position)
                                 .OrderBy(node => Guid.NewGuid())
                                 .ToList();

            if (CanSeePlayerInUnobstructedLine || (CanSeeActivityInUnobstructedLine && Owner.CurrentRegion.HasDestructibleActivity)) { NextAction = Action.Fire; } else {
                if (Path != null && !Path.Any()) Path = null;

                if (CurrentPatrolRoute is null || !CurrentPatrolRoute.Any()) CurrentPatrolRoute = new(PatrolRoute);

                if (Path == null || TargetPosition.IsInvalid() || TargetPosition.Equals(Owner.CurrentRegion.ConvertMapCoordsToRegionFieldCoords(Owner.Coords))) {
                    SetCooldown(0.05);
                    TargetPosition = CurrentPatrolRoute.Pop();
                    Path ??= GeneratePath(Owner.CurrentRegion.GetNodesInRegion(), Owner.CurrentRegion.ConvertMapCoordsToRegionFieldCoords(Owner.Coords), TargetPosition);
                }

                NextAction = Path == null ? Action.Nothing : GetActionFromNextCoords(
                    Path.Pop()
                        .Position + Owner.CurrentRegion.Coords * Owner.CurrentRegion.FieldsInLine
                );
            }
        } else { NextAction = Action.Nothing; }
    }
}