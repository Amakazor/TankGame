using System;
using System.Numerics;
using LanguageExt;
using TankGame.Actors.Brains.Thoughts;
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

    public override Option<Thought> NextThought() {
        (var action, NextAction) = (NextAction, null);
        
        return action switch {
            InputAction.MoveForward => MoveForward(),
            InputAction.RotateLeft  => new RotateThought(Brain, 1.0f, -90),
            InputAction.RotateRight => new RotateThought(Brain, 1.0f,  90),
            InputAction.Shoot       => new ShootThought (Brain, 1.0f),
            _                       => None,
        };
    }

    private Option<Thought> MoveForward()
        => GamestateManager.Map.GetFieldFromRegion(Brain.Owner.Coords).SelectMany(_ => GamestateManager.Map.GetFieldFromRegion(Brain.Owner.Coords + Brain.Owner.Direction.ToIVector()), (currentField, targetField) => new MoveThought(Brain, 1.0f, currentField, targetField)).Map<Thought>(t => t);
}