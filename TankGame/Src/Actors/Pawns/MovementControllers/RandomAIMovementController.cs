using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Data.Controls;
using TankGame.Src.Extensions;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal class RandomAIMovementController : AIMovementController
    {
        protected List<Vector2i> PatrolRoute { get; set; }
        protected Stack<Vector2i> CurrentPatrolRoute { get; set; }

        public RandomAIMovementController(double delay, Pawn owner) : base(delay, owner, "random")
        {
            TargetPosition = new Vector2i(-1, -1);
        }

        protected override void DecideOnNextAction()
        {
            if (CanDoAction() && Owner.CurrentRegion != null)
            {
                PatrolRoute ??= Owner.CurrentRegion.GetNodesInRegion().SelectMany(node => node).ToList().FindAll(node => node.Walkable).Select(node => node.Position).OrderBy(node => Guid.NewGuid()).ToList();

                if (CanSeePlayerInUnobstructedLine || (CanSeeActivityInUnobstructedLine && Owner.CurrentRegion.HasDestructibleActivity)) NextAction = KeyActionType.Shoot;
                else
                {
                    if (CurrentPatrolRoute is null || CurrentPatrolRoute.Count == 0)
                    {
                        CurrentPatrolRoute = new Stack<Vector2i>(PatrolRoute);
                    }

                    if (Path != null && !Path.Any()) Path = null;

                    while (CurrentPatrolRoute != null && CurrentPatrolRoute.Any() && (Path == null || TargetPosition.IsInvalid() || TargetPosition.Equals(Owner.CurrentRegion.ConvertMapCoordsToRegionFieldCoords(Owner.Coords))))
                    {
                        TargetPosition = CurrentPatrolRoute.Pop();
                        Path ??= GeneratePath(Owner.CurrentRegion.GetNodesInRegion(), Owner.CurrentRegion.ConvertMapCoordsToRegionFieldCoords(Owner.Coords), TargetPosition);
                    }

                    NextAction = Path == null ? null : GetActionFromNextCoords(Path.Pop().Position + Owner.CurrentRegion.Coords * Owner.CurrentRegion.FieldsInLine);
                }
            }
            else NextAction = null;
        }
    }
}
