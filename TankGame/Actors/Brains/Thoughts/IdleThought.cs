using TankGame.Core;

namespace TankGame.Actors.Brains.Thoughts; 

public class IdleThought : Thought, IDto<IdleThought.Dto>
{
    public new class Dto : Thought.Dto { }
    
    public IdleThought(Brain brain, float totalTime) : base(brain, totalTime) { }
    public IdleThought(Brain brain, Dto dto) : base(brain, dto) { }
    public override void Initialize() {}
    public override void FinishThought() {}

    public override Dto ToDto()
        => new() {
            TimeLeft = TimeLeft, 
            TotalTime = TotalTime,
        };
}