using System;
using TankGame.Actors.Brains.Thoughts;

namespace TankGame.Actors.Brains.Goals; 

public abstract class Goal {
    protected Goal(Brain brain) {
        Brain = brain;
    }
    
    protected Guid Id { get; set; } = Guid.NewGuid();
    protected Brain Brain { get; private set; }
    
    public virtual Thought? NextThought() 
        => null;
}