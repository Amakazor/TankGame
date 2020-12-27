using SFML.System;
using System.Collections.Generic;
using TankGame.Src.Data;
using TankGame.Src.Extensions;
using TankGame.Src.Pathfinding;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    class StandGroundAIMovementController : ChaseAIMovementController
    {
        private Vector2i HomePosition { get; }
        private const int StandGroundRadius = 2;
        public StandGroundAIMovementController(float delay, Pawn owner) : base(delay, owner)
        {
            HomePosition = Owner.Coords;
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
                        TargetPosition = new Vector2i(-1, -1);
                        Path = null;
                    }

                    if (TargetPosition.Equals(new Vector2i(-1, -1)) || !GamestateManager.Instance.Map.GetFieldFromRegion(TargetPosition).IsTraversible())
                    {
                        TargetPosition = GetValidStandGroundPosition();
                        Path = null;
                    }

                    if (!TargetPosition.Equals(new Vector2i(-1, -1)) && GamestateManager.Instance.Map.GetFieldFromRegion(TargetPosition).IsTraversible())
                    {
                        if ((Path == null || Path.Count == 0) && (IsPlayerWithinChaseRadius() || TargetPosition.Equals(HomePosition)))
                        {
                            AStar aStar = new AStar(GamestateManager.Instance.Map.GetNodesInRadius(Owner.Coords, SightDistance));
                            Path = aStar.FindPath(new Vector2i(SightDistance, SightDistance), new Vector2i(SightDistance, SightDistance) + TargetPosition - Owner.Coords);
                        }

                        if ((IsPlayerWithinChaseRadius() || TargetPosition.Equals(HomePosition)) && Path != null)
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

        private Vector2i GetValidStandGroundPosition()
        {
            Vector2i targetPosition = GetClosestValidShootingPosition(GetValidShootingPositions().FindAll(position => position.ManhattanDistance(HomePosition) <= StandGroundRadius));
            return targetPosition.Equals(new Vector2i(-1, -1)) ? HomePosition : targetPosition;
        }
    }
}
