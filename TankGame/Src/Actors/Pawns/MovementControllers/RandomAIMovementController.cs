using TankGame.Src.Data;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal class RandomAIMovementController : AIMovementController
    {
        public RandomAIMovementController(double delay, Pawn owner) : base(delay, owner, "random")
        {
        }

        protected override void DecideOnNextAction()
        {
            if (CanDoAction() && Owner.CurrentRegion != null)
            {
                if (CanSeePlayerInUnobstructedLine || CanSeeActivityInUnobstructedLine) NextAction = KeyActionType.Shoot;
                else
                {
                    NextAction = (GamestateManager.Instance.Random.Next(1, 5)) switch
                    {
                        1 => KeyActionType.MoveUp,
                        2 => KeyActionType.MoveDown,
                        3 => KeyActionType.MoveLeft,
                        4 => KeyActionType.MoveRight,
                        _ => null,
                    };
                }
            }
            else NextAction = null;
        }
    }
}
