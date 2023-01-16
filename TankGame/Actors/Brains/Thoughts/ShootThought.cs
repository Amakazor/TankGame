using TankGame.Actors.Projectiles;
using TankGame.Core.Sounds;

namespace TankGame.Actors.Brains.Thoughts; 

public class ShootThought : Thought, IDto<ShootThought.Dto> {
    public new class Dto : Thought.Dto { }
    public ShootThought(Brain brain, Dto dto) : base(brain, dto) { }
    public ShootThought(Brain brain, float totalTime) : base(brain, totalTime) { }

    public override void Initialize()
        => Projectile.Create(Brain.Owner);
    public override void FinishThought() {}
    
    public Dto ToDto()
        => new Dto() {
            TotalTime = TotalTime,
            TimeLeft = TimeLeft,
        };
}