using System;
using TankGame.Src.Actors.Projectiles;
using TankGame.Src.Data;
using TankGame.Src.Events;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal class PlayerMovementController : MovementController
    {
        public PlayerMovementController(float delay, Pawn owner) : base(delay, owner)
        {
            MessageBus.Instance.Register(MessageType.KeyAction, SetNextAction);
        }

        public override bool CanDoAction()
        {
            return base.CanDoAction() && NextAction != null;
        }

        private void SetNextAction(object sender, EventArgs eventArgs)
        {
            if (eventArgs is KeyActionEventArgs keyActionEventArgs && keyActionEventArgs.KeyActionType != null) NextAction = keyActionEventArgs.KeyActionType;
        }
    }
}
