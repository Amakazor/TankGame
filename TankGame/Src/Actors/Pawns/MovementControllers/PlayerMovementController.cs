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
            return NextAction != null ? base.CanDoAction() : false;
        }

        private void SetNextAction(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Player movement controller received action");

            if (eventArgs is KeyActionEventArgs && ((KeyActionEventArgs)eventArgs).KeyActionType != null)
            {
                Console.WriteLine("\tAction type: " + ((KeyActionEventArgs)eventArgs).KeyActionType.Item2);
                NextAction = ((KeyActionEventArgs)eventArgs).KeyActionType;
            }
        }
    }
}
