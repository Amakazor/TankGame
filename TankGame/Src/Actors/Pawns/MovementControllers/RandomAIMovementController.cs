using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Data;
using TankGame.Src.Data.Controls;
using TankGame.Src.Extensions;
using TankGame.Src.Pathfinding;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal class RandomAIMovementController : AIMovementController
    {
        protected List<Vector2i> PatrolRoute { get; set; }
        protected Stack<Vector2i> CurrentPatrolRoute { get; set; }
        protected Vector2i TargetPosition { get; set; }
        protected Stack<Node> Path { get; set; }

        public RandomAIMovementController(double delay, Pawn owner) : base(delay, owner, "random")
        {
            TargetPosition = new Vector2i(-1, -1);
        }

        protected override void DecideOnNextAction()
        {
            if (CanDoAction() && Owner.CurrentRegion != null)
            {
                PatrolRoute ??= Owner.CurrentRegion.GetNodesInRegion().SelectMany(node => node).Select(node => node.Position).OrderBy(node => Guid.NewGuid()).ToList();

                if (CanSeePlayerInUnobstructedLine || (CanSeeActivityInUnobstructedLine && Owner.CurrentRegion.HasDestructibleActivity)) NextAction = KeyActionType.Shoot;
                else
                {
                    if (CurrentPatrolRoute is null || CurrentPatrolRoute.Count == 0)
                    {
                        CurrentPatrolRoute = new Stack<Vector2i>(PatrolRoute);
                    }

                    if (TargetPosition.IsInvalid() || TargetPosition.Equals(Owner.Coords))
                    {
                        TargetPosition = CurrentPatrolRoute.Pop();
                    }

                    if (!TargetPosition.IsInvalid())
                    {
                        if (Path != null && Path.Count == 0) Path = null;

                        Path ??= GeneratePath(Owner.CurrentRegion.GetNodesInRegion(), Owner.CurrentRegion.ConvertMapCoordsToRegionFieldCoords(Owner.Coords), TargetPosition);

                        NextAction = Path == null ? null : GetActionFromNextCoords(Path.Pop().Position + Owner.CurrentRegion.Coords * 20);
                    }
                    else NextAction = null;
                }
            }
            else NextAction = null;
        }
    }
}
