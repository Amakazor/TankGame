using System;
using System.Numerics;
using SFML.System;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.Fields;
using TankGame.Actors.Pawns;
using TankGame.Core.Controls;
using TankGame.Core.Gamestate;
using TankGame.Events;

namespace TankGame.Actors.Brains.Goals; 

public class BePlayerGoal : Goal {
    public BePlayerGoal(Brain brain) : base(brain) {
        MessageBus.KeyAction += SetNextAction;
    }

    ~BePlayerGoal() {
        MessageBus.KeyAction -= SetNextAction;
    }

    public InputAction? NextAction { get; private set; }

    private void SetNextAction(InputAction inputAction) {
        if (Brain.CurrentThought?.TimeLeft > 0.2f) return;
        NextAction = inputAction;
    }

    public override Thought? NextThought() {
        if (NextAction == InputAction.Shoot) {
            Console.WriteLine("Shooting");
        }
        
        (var action, NextAction) = (NextAction, null);
        
        return action switch {
            InputAction.MoveForward => MoveForward(),
            InputAction.RotateLeft  => new RotateThought(Brain, 1.0f, -90),
            InputAction.RotateRight => new RotateThought(Brain, 1.0f,  90),
            InputAction.Shoot       => new ShootThought (Brain, 1.0f),
            _                       => null,
        };
    }

    private Thought? MoveForward() {
        Field? currentField = GamestateManager.Map.GetFieldFromRegion(Brain.Owner.Coords);
        Field? targetField = GamestateManager.Map.GetFieldFromRegion(Brain.Owner.Coords + Brain.Owner.Direction.ToIVector());
        return currentField == null || targetField == null || !targetField.IsTraversible() ? null : new MoveThought(Brain, 1.0f, currentField, targetField);
    }
}