using LanguageExt;
using TankGame.Actors.Brains.Thoughts;

namespace TankGame.Actors.Brains.Goals; 

public class IdleGoal : Goal {
    public IdleGoal(Brain brain) : base(brain) { }
    public override Option<Thought> NextThought()
        => new IdleThought(Brain, 0.2f);
}