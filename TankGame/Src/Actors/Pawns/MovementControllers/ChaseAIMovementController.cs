using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Data;
using TankGame.Src.Pathfinding;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal class ChaseAIMovementController : AIMovementController
    {
        private Vector2i LastPlayerPosition;
        private Vector2i TargetPosition;
        private Stack<Node> Path;
        public ChaseAIMovementController(float delay, Pawn owner) : base(delay, owner)
        {
        }

        protected override void DecideOnNextAction()
        {
            if (CanDoAction())
            {
                if (CanSeePlayerInUnobstructedLine) NextAction = KeyActionType.Shoot;
                else
                {
                    Vector2i currentPlayerPosition = GamestateManager.Instance.Player.Coords;

                    if (LastPlayerPosition == null || LastPlayerPosition != currentPlayerPosition)
                    {
                        LastPlayerPosition = currentPlayerPosition;
                        TargetPosition = GetClosestValidShootingPosition();
                        Path = null;
                    }

                    if (!TargetPosition.Equals(new Vector2i(-1, -1)) && !GamestateManager.Instance.Map.GetFieldFromRegion(TargetPosition).IsTraversible())
                    {
                        TargetPosition = GetClosestValidShootingPosition();
                        Path = null;
                    }

                    if (!TargetPosition.Equals(new Vector2i(-1, -1)))
                    {
                        if ((Path == null || Path.Count == 0) && IsPlayerWithinChaseRadius())
                        {
                            AStar aStar = new AStar(GamestateManager.Instance.Map.GetNodesInRadius(Owner.Coords, SightDistance));
                            Path = aStar.FindPath(new Vector2i(SightDistance, SightDistance), new Vector2i(SightDistance, SightDistance) + TargetPosition - Owner.Coords);
                        }

                        if (IsPlayerWithinChaseRadius() && Path != null)
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

        private bool IsPlayerWithinChaseRadius()
        {
            Vector2i PlayerCoords = GamestateManager.Instance.Player.Coords;
            Vector2i OwnerCoords = Owner.Coords;

            return Math.Abs(PlayerCoords.X - OwnerCoords.X) <= SightDistance && Math.Abs(PlayerCoords.Y - OwnerCoords.Y) <= SightDistance;
        }
    }
}
