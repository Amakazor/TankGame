using System;
using TankGame.Actors.Pawns;
using TankGame.Core;
using TankGame.Core.Sounds;
using TankGame.Utility;

namespace TankGame.Actors.Brains.Thoughts; 
 
public class RotateThought : Thought, IDto<RotateThought.Dto> {
    public new class Dto : Thought.Dto {
        public Direction NextDirection { get; set; }
        public float BaseAngle { get; set; }
        public float TargetAngle { get; set; }
        
        public void Deconstruct(out Direction nextDirection, out float baseAngle, out float targetAngle) {
            nextDirection = NextDirection;
            baseAngle = BaseAngle;
            targetAngle = TargetAngle;
        }
    }
    
    public RotateThought(Brain brain, Dto dto) : base(brain, dto) {
        (NextDirection, BaseAngle, TargetAngle) = dto;
    }
    public RotateThought(Brain brain, float totalTime, Direction nextDirection) 
        : base(brain, totalTime * Math.Abs(brain.Owner.Direction.ToAngle() - GetBestAngle(brain.Owner.Direction.ToAngle(), nextDirection.ToAngle())) / 90) {
        NextDirection = nextDirection;
        if (NextDirection == Brain.Owner.Direction) TimeLeft = 0.0F;

        BaseAngle = Brain.Owner.Direction.ToAngle();
        TargetAngle = GetBestAngle(brain.Owner.Direction.ToAngle(), NextDirection.ToAngle());
    }

    public RotateThought(Brain brain, float totalTime, float rotationAngle) 
        : base(brain, totalTime * Math.Abs(rotationAngle) / 90) {
        NextDirection = Brain.Owner.Direction.RotateByAngle(rotationAngle);
        if (NextDirection == Brain.Owner.Direction) TimeLeft = 0.0F;

        BaseAngle = Brain.Owner.Direction.ToAngle();
        TargetAngle = BaseAngle + rotationAngle;
    }

    private Direction NextDirection { get; }
    private float BaseAngle { get; }
    private float TargetAngle { get; }
    private float CurrentAngle => MathHelper.Lerp(BaseAngle, TargetAngle, Completion) % 360;

    public static float GetBestAngle(float baseAngle, float targetAngle) {
        if (Math.Abs(baseAngle - 270) < float.Epsilon && Math.Abs(targetAngle)       < float.Epsilon) return 360;
        if (Math.Abs(baseAngle)       < float.Epsilon && Math.Abs(targetAngle - 270) < float.Epsilon) return -90;
        return targetAngle;
    }

    public override void Initialize() {}
    
    public override void Tick(float deltaTime) {
        base.Tick(deltaTime, false);
        Brain.Owner.SetRotation(CurrentAngle);
        CheckForCompletion();
    }

    public override void FinishThought() {
        Brain.Owner.Direction = NextDirection;
        Brain.Owner.SetRotation(NextDirection.ToAngle());
    }

    public Dto ToDto()
        => new(){
            NextDirection = NextDirection,
            BaseAngle = BaseAngle,
            TargetAngle = TargetAngle,
            TimeLeft = TimeLeft,
            TotalTime = TotalTime,
        };
}