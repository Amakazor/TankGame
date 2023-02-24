using System;
using System.Text.Json.Serialization;
using LanguageExt;
using TankGame.Actors.Brains.Thoughts;

namespace TankGame.Actors.Brains.Goals; 

public abstract class Goal {
    [JsonDerivedType(typeof(BePlayerGoal.Dto),    typeDiscriminator: nameof(BePlayerGoal))]
    [JsonDerivedType(typeof(ChasePlayerGoal.Dto), typeDiscriminator: nameof(ChasePlayerGoal))]
    [JsonDerivedType(typeof(ChaseTowerGoal.Dto),  typeDiscriminator: nameof(ChaseTowerGoal))]
    [JsonDerivedType(typeof(IdleGoal.Dto),        typeDiscriminator: nameof(IdleGoal))]
    [JsonDerivedType(typeof(RandomWalkGoal.Dto),  typeDiscriminator: nameof(RandomWalkGoal))]
    [JsonDerivedType(typeof(ShootPlayerGoal.Dto), typeDiscriminator: nameof(ShootPlayerGoal))]
    [JsonDerivedType(typeof(ShootTowerGoal.Dto),  typeDiscriminator: nameof(ShootTowerGoal))]
    public class Dto {
        public Dto() { }
        public Dto(Guid id)
            => Id = id;
        public Guid Id { get; set; }
    }
    protected Goal(Brain brain) {
        Brain = brain;
    }
    
    protected Goal(Brain brain, Dto dto) {
        Brain = brain;
        Id = dto.Id;
    }
    
    protected Guid Id { get; set; } = Guid.NewGuid();
    protected Brain Brain { get; private set; }
    
    public virtual Option<Thought> NextThought() 
        => None;
    
    public virtual Dto ToDto() 
        => new() { Id = Id };
}