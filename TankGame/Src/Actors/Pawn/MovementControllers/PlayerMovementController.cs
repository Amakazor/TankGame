using System;
using TankGame.Src.Events;

namespace TankGame.Src.Actors.Pawn.MovementControllers
{
    class PlayerMovementController : MovementController
    {
        public PlayerMovementController(float delay, Pawn owner) : base(delay, owner)
        {
            MessageBus.Instance.Register(MessageType.KeyAction, SetNextAction);
        }

        private void SetNextAction(object sender, EventArgs eventArgs)
        {
            if (eventArgs is KeyActionEventArgs && ((KeyActionEventArgs)eventArgs).KeyActionType != null)
            {
                NextAction = ((KeyActionEventArgs)eventArgs).KeyActionType;
            }
        }
    }
}
