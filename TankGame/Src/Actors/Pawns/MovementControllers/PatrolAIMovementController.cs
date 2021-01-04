using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Data;
using TankGame.Src.Extensions;
using TankGame.Src.Pathfinding;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal class PatrolAIMovementController : AIMovementController
    {
        protected List<Vector2i> PatrolRoute { get; }
        protected Stack<Vector2i> CurrentPatrolRoute { get; set; }
        protected Vector2i TargetPosition { get; set; }
        protected Stack<Node> Path { get; set; }

        public PatrolAIMovementController(double delay, Pawn owner, List<Vector2i> patrolRoute) : base(delay, owner)
        {
            PatrolRoute = patrolRoute;
            TargetPosition = new Vector2i(-1, -1);
        }

        protected override void DecideOnNextAction()
        {
            if (CanDoAction())
            {
                if (CanSeePlayerInUnobstructedLine || CanSeeActivityInUnobstructedLine) NextAction = KeyActionType.Shoot;
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

                        Path ??= GeneratePath(GamestateManager.Instance.Map.GetNodesInRadius(Owner.Coords, BaseSightDistance), new Vector2i(BaseSightDistance, BaseSightDistance), new Vector2i(BaseSightDistance, BaseSightDistance) + TargetPosition - Owner.Coords);

                        NextAction = Path == null ? null : GetActionFromNextCoords(Path.Pop().Position + Owner.Coords - new Vector2i(BaseSightDistance, BaseSightDistance));
                    }
                    else NextAction = null;
                }
            }
            else NextAction = null;
        }
    }
}
