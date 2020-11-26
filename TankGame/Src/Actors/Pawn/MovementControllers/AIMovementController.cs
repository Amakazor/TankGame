namespace TankGame.Src.Actors.Pawn.MovementControllers
{
    internal abstract class AIMovementController : MovementController
    {
        public AIMovementController(float delay, Pawn owner) : base(delay, owner)
        {
        }

        public override Direction DoAction(Direction currentDirection)
        {
            if (NextAction == null)
            {
                DecideOnNextAction();
            }

            return base.DoAction(currentDirection);
        }

        protected abstract void DecideOnNextAction();
    }
}
