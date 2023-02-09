using TankGame.Actors.Projectiles;
using TankGame.Core;

namespace TankGame.Actors.Brains.Thoughts; 

public class ShootThought : Thought, IDto<ShootThought.Dto> {
    public new class Dto : Thought.Dto { }
    public ShootThought(Brain brain, Dto dto) : base(brain, dto) { }
    public ShootThought(Brain brain, float totalTime) : base(brain, totalTime) { }

    public override void Initialize()
        => Projectile.Create(Brain.Owner);
    public override void FinishThought() {}
    
    public override Dto ToDto()
        => new() {
            TotalTime = TotalTime,
            TimeLeft = TimeLeft,
        };
}