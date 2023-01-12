using System.Text.Json.Serialization;
using TankGame.Core.Controls;
using TankGame.Events;

namespace TankGame.Actors.Pawns.MovementControllers;

public class PlayerMovementController : MovementController {
    public PlayerMovementController(float delay, Pawn owner) : base(delay, owner)
        => MessageBus.Action += SetNextAction;

    [JsonConstructor] public PlayerMovementController(double delay, Action nextAction, double rotationCooldown, double movementCooldown) : base(delay) {
        MessageBus.Action += SetNextAction;

        NextAction = nextAction;
        RotationCooldown = rotationCooldown;
        MovementCooldown = movementCooldown;
    }

    public override bool CanDoAction()
        => base.CanDoAction() && NextAction != null;

    private void SetNextAction(Action keyAction) {
        if (keyAction == Action.Pause) return;
        NextAction = keyAction;
    }
}