using TankGame.Core.Sounds;

namespace TankGame.Actors.Brains.Thoughts; 

public class IdleThought : Thought, IDto<IdleThought.Dto>
{
    public new class Dto : Thought.Dto { }
    public IdleThought(Brain brain, float totalTime) : base(brain, totalTime) { }
    public override void Initialize() {}
    public override void FinishThought() {}

    public Dto ToDto()
        => new Dto() { TimeLeft = TimeLeft, TotalTime = TotalTime, };
}