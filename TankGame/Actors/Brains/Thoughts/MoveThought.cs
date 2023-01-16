using SFML.System;
using TankGame.Actors.Fields;
using TankGame.Actors.Pawns.Player;
using TankGame.Core.Gamestate;
using TankGame.Core.Sounds;
using TankGame.Events;
using TankGame.Utility;

namespace TankGame.Actors.Brains.Thoughts;

public class MoveThought : Thought, IDto<MoveThought.Dto> {
    
    public new class Dto : Thought.Dto {
        public Vector2i BaseField { get; set; }
        public Vector2i TargetField { get; set; }
        public bool AlreadyAchievedSecondPoint { get; set; }
        public bool AlreadyAchievedFifthPoint { get; set; }
        public bool AlreadyAchievedEightPoint { get; set; }
        
        public void Deconstruct(out Field baseField, out Field targetField, out bool alreadyAchievedSecondPoint, out bool alreadyAchievedFifthPoint, out bool alreadyAchievedEightPoint) {
            baseField = GamestateManager.Map.GetFieldFromRegion(BaseField)!;
            targetField = GamestateManager.Map.GetFieldFromRegion(TargetField)!;
            alreadyAchievedSecondPoint = AlreadyAchievedSecondPoint;
            alreadyAchievedFifthPoint = AlreadyAchievedFifthPoint;
            alreadyAchievedEightPoint = AlreadyAchievedEightPoint;
        }
    }
    
    public MoveThought(Brain brain, float totalTime, Field baseField, Field targetField) : base(brain, totalTime) {
        BaseField = baseField;
        TargetField = targetField;
    }
    
    public MoveThought(Brain brain, Dto dto) : base(brain, dto) {
        (BaseField, TargetField, AlreadyAchievedSecondPoint, AlreadyAchievedFifthPoint, AlreadyAchievedEightPoint) = dto;
    }
    
    private Field BaseField { get; }
    private Field TargetField { get; }
    private Vector2f CurrentPosition => MathHelper.Lerp(BaseField.Position, TargetField.Position, Completion);
    private bool AlreadyAchievedSecondPoint { get; set; }
    private bool AlreadyAchievedFifthPoint { get; set; }
    private bool AlreadyAchievedEightPoint { get; set; }

    public override void Initialize() {
        TargetField.PawnOnField = Brain.Owner;
    }

    public override void Tick(float deltaTime) {
        Brain.Owner.SetPosition(CurrentPosition);
        base.Tick(deltaTime, false);
        
        if (Brain.Owner is Player player) MessageBus.PlayerMoved(player);
        
        if (!AlreadyAchievedSecondPoint && Completion > 0.2f) {
            AlreadyAchievedSecondPoint = true;
            TargetField.DestroyObjectIfExists();
        }
        
        if (!AlreadyAchievedFifthPoint && Completion >= 0.5f) {
            AlreadyAchievedFifthPoint = true;
            Brain.Owner.Coords = TargetField.Coords;
        }

        if (!AlreadyAchievedEightPoint && Completion >= 0.8f) {
            AlreadyAchievedEightPoint = true;
            BaseField.PawnOnField = null;
        }
        
        CheckForCompletion();
    }

    public override void FinishThought() {}
    
    public Dto ToDto()
        => new Dto {
            TotalTime = TotalTime,
            TimeLeft = TimeLeft,
            BaseField = BaseField.Coords,
            TargetField = TargetField.Coords,
            AlreadyAchievedSecondPoint = AlreadyAchievedSecondPoint,
            AlreadyAchievedFifthPoint = AlreadyAchievedFifthPoint,
            AlreadyAchievedEightPoint = AlreadyAchievedEightPoint,
        };
}