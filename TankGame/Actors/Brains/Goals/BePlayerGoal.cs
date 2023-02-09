using LanguageExt;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.Pawns;
using TankGame.Core.Controls;
using TankGame.Core.Gamestates;
using TankGame.Events; 

namespace TankGame.Actors.Brains.Goals; 

public class BePlayerGoal : Goal {
    public new class Dto : Goal.Dto {
        public InputAction NextAction { get; set; }
    }
    
    public BePlayerGoal(Brain brain) : base(brain) {
        MessageBus.KeyAction += SetNextAction;
    }
    
    public BePlayerGoal(Brain brain, Dto dto) : base(brain, dto) {
        NextAction = dto.NextAction;
        MessageBus.KeyAction += SetNextAction;
    }

    ~BePlayerGoal() {
        MessageBus.KeyAction -= SetNextAction;
    }

    public InputAction NextAction { get; private set; } = InputAction.Nothing;

    private void SetNextAction(InputAction inputAction) {
        if (Brain.CurrentThought.Match(thought => thought.TimeLeft, 0.0f) > 0.2f) return;
        NextAction = inputAction;
    }

    public override Option<Thought> NextThought() {
        (var action, NextAction) = (NextAction, InputAction.Nothing);
        
        return action switch {
            InputAction.MoveForward => MoveForward(),
            InputAction.RotateLeft  => new RotateThought(Brain, 1.0f, -90),
            InputAction.RotateRight => new RotateThought(Brain, 1.0f,  90),
            InputAction.Shoot       => new ShootThought (Brain, 1.0f),
            _                       => None,
        };
    }
    
    public override Dto ToDto()
        => new() { Id = Id, NextAction = NextAction, };

    private Option<Thought> MoveForward()
        => Gamestate.Level.FieldAt(Brain.Owner.Coords)
                    .Bind(currentField => Gamestate.Level.FieldAt(Brain.Owner.Coords + Brain.Owner.Direction.ToIVector()).Map(targetField => (currentField, targetField)))
                    .Filter(fields => fields.targetField.Traversible)
                    .Map<Thought>(fields => new MoveThought(Brain, Brain.Delay * ((fields.currentField.SpeedModifier + fields.targetField.SpeedModifier)/2), fields.currentField, fields.targetField));
}