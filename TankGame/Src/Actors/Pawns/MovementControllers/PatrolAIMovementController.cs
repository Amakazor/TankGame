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
                if (CanSeePlayerInUnobstructedLine) NextAction = KeyActionType.Shoot;
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

                    if (!TargetPosition.Equals(new Vector2i(-1, -1)))
                    {
                        if (Path == null || Path.Count == 0)
                        {
                            AStar aStar = new AStar(GamestateManager.Instance.Map.GetNodesInRadius(Owner.Coords, SightDistance));
                            Path = aStar.FindPath(new Vector2i(SightDistance, SightDistance), new Vector2i(SightDistance, SightDistance) + TargetPosition - Owner.Coords);
                        }

                        if (Path != null)
                        {
                            Node node = Path.Pop();

                            if (node != null)
                            {
                                Vector2i nextCoords = node.Position + Owner.Coords - new Vector2i(SightDistance, SightDistance);

                                NextAction = GetActionFromNextCoords(nextCoords);
                            }
                            else NextAction = null;
                        }
                        else NextAction = null;
                    }
                    else NextAction = null;
                }
            }
            else NextAction = null;
        }
    }
}
