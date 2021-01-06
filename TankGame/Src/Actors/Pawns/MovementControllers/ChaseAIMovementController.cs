using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Data;
using TankGame.Src.Extensions;
using TankGame.Src.Pathfinding;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal class ChaseAIMovementController : AIMovementController
    {
        protected Vector2i LastPlayerPosition { get; set; }
        protected Vector2i TargetPosition { get; set; }
        protected Stack<Node> Path { get; set; }
        public ChaseAIMovementController(double delay, Pawn owner, string type = null) : base(delay, owner, type??"chase")
        {
        }

        protected override void DecideOnNextAction()
        {
            if (CanDoAction())
            {
                if (CanSeePlayerInUnobstructedLine || (CanSeeActivityInUnobstructedLine && Owner.CurrentRegion != null && Owner.CurrentRegion.HasDestructibleActivity)) NextAction = KeyActionType.Shoot;
                else
                {
                    if (Owner.CurrentRegion != null && Owner.CurrentRegion.HasDestructibleActivity) ChaseActivity();
                    else ChasePlayer();
                }
            }
            else NextAction = null;
        }

        protected bool IsPlayerWithinChaseRadius()
        {
            Vector2i PlayerCoords = GamestateManager.Instance.Player.Coords;
            Vector2i OwnerCoords = Owner.Coords;

            return Math.Abs(PlayerCoords.X - OwnerCoords.X) <= SightDistance && Math.Abs(PlayerCoords.Y - OwnerCoords.Y) <= SightDistance;
        }

        protected void ChasePlayer()
        {
            if (Owner.CurrentRegion != null)
            {
                if (!IsPlayerWithinChaseRadius()) NextAction = null;
                else
                {
                    Vector2i currentPlayerPosition = GamestateManager.Instance.Player.Coords;

                    if (LastPlayerPosition == null || LastPlayerPosition != currentPlayerPosition || !GamestateManager.Instance.Map.GetFieldFromRegion(TargetPosition).IsTraversible())
                    {
                        LastPlayerPosition = currentPlayerPosition;
                        TargetPosition = GetClosestValidShootingPositionToPlayer();
                        Path = null;
                    }

                    if (!TargetPosition.IsInvalid())
                    {
                        if (Path != null && Path.Count == 0) Path = null;

                        Path ??= GeneratePath(GamestateManager.Instance.Map.GetNodesInRadius(Owner.Coords, SightDistance), new Vector2i(SightDistance, SightDistance), new Vector2i(SightDistance, SightDistance) + TargetPosition - Owner.Coords);

                        NextAction = Path == null ? null : GetActionFromNextCoords(Path.Pop().Position + Owner.Coords - new Vector2i(SightDistance, SightDistance));
                    }
                    else NextAction = null;
                }
            }
            else NextAction = null;

        }

        protected void ChaseActivity()
        {
            if (!TargetPosition.Equals(new Vector2i(-1, -1)))
            {
                if (Path != null && Path.Count == 0) Path = null;

                Path ??= GeneratePath(Owner.CurrentRegion.GetNodesInRegion(), Owner.CurrentRegion.ConvertMapCoordsToRegionFieldCoords(Owner.Coords), GetClosestValidShootingPositionToActivity());

                NextAction = Path == null ? null : GetActionFromNextCoords(Path.Pop().Position);
            }
            else NextAction = null;
        }
    }
}
