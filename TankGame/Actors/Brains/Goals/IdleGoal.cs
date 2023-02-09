using LanguageExt;
using TankGame.Actors.Brains.Thoughts;

namespace TankGame.Actors.Brains.Goals; 

public class IdleGoal : Goal {
    public new class Dto : Goal.Dto { }
    
    public IdleGoal(Brain brain) : base(brain) { }
    public IdleGoal(Brain brain, Dto dto) : base(brain, dto) { }
    public override Option<Thought> NextThought()
        => new IdleThought(Brain, 0.2f);
}