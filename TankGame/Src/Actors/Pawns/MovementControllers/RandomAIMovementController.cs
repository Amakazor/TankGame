using TankGame.Src.Data;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    class RandomAIMovementController : AIMovementController
    {
        public RandomAIMovementController(float delay, Pawn owner) : base(delay, owner)
        {
        }

        protected override void DecideOnNextAction()
        {
            if (CanDoAction())
            {
                if (CanSeePlayerInLine())
                {
                    NextAction = KeyActionType.Shoot;
                }
                else
                {
                    switch (Utility.GetRandomInt(1, 5))
                    {
                        case 1:  NextAction = KeyActionType.MoveUp;    break;
                        case 2:  NextAction = KeyActionType.MoveDown;  break;
                        case 3:  NextAction = KeyActionType.MoveLeft;  break;
                        case 4:  NextAction = KeyActionType.MoveRight; break;
                        default: NextAction = null; break;
                    }
                }
            }
            else
            {
                NextAction = null;
            }
        }
    }
}
