using SFML.System;
using TankGame.Src.Data.Controls;
using TankGame.Src.Data.Gamestate;
using TankGame.Src.Extensions;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    class StandGroundAIMovementController : ChaseAIMovementController
    {
        private Vector2i HomePosition { get; }
        private const int StandGroundRadius = 3;
        public StandGroundAIMovementController(double delay, Pawn owner) : base(delay, owner, "stand")
        {
            HomePosition = Owner.Coords;
        }

        protected override void DecideOnNextAction()
        {
            if (CanDoAction() && Owner.CurrentRegion != null)
            {
                if (CanSeePlayerInUnobstructedLine || (CanSeeActivityInUnobstructedLine && Owner.CurrentRegion != null && Owner.CurrentRegion.HasDestructibleActivity)) NextAction = KeyActionType.Shoot;
                else
                {
                    Vector2i currentPlayerPosition = GamestateManager.Instance.Player.Coords;

                    if (LastPlayerPosition != currentPlayerPosition || !GamestateManager.Instance.Map.GetFieldFromRegion(TargetPosition).IsTraversible())
                    {
                        LastPlayerPosition = currentPlayerPosition;
                        TargetPosition = GetValidStandGroundPosition();
                        Path = null;
                    }

                    if (!IsPlayerWithinChaseRadius() && !TargetPosition.Equals(HomePosition)) NextAction = null;
                    else
                    {
                        if (!TargetPosition.IsInvalid())
                        {
                            if (Path != null && Path.Count == 0) Path = null;

                            Path ??= GeneratePath(GamestateManager.Instance.Map.GetNodesInRadius(Owner.Coords, SightDistance), new Vector2i(SightDistance, SightDistance), new Vector2i(SightDistance, SightDistance) + TargetPosition - Owner.Coords);

                            NextAction = Path == null ? null : GetActionFromNextCoords(Path.Pop().Position + Owner.Coords - new Vector2i(SightDistance, SightDistance));
                        }
                        else NextAction = null;
                    }
                }
            }
            else NextAction = null;
        }

        private Vector2i GetValidStandGroundPosition()
        {
            Vector2i targetPosition = GetClosestValidShootingPositionToPlayer(GetValidShootingPositions().FindAll(position => position.ManhattanDistance(HomePosition) <= StandGroundRadius));
            return targetPosition.Equals(new Vector2i(-1, -1)) ? HomePosition : targetPosition;
        }
    }
}
