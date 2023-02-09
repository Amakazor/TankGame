using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using LanguageExt;
using TankGame.Actors.Brains.Goals;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.Pawns;

namespace TankGame.Actors.Brains; 

public class Brain : IDisposable {
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global"), SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class Dto {
        [JsonIgnore] public Option<Thought.Dto> CurrentThoughtDto { get; set; }
        public Thought.Dto? CurrentThought {
            get => CurrentThoughtDto.MatchUnsafe(t => t, () => null);
            set => CurrentThoughtDto = Optional(value);
        }
        
        public Dictionary<int, Goal.Dto> Goals { get; set; }
        public float Delay { get; set; }
    }
    public Brain(Pawn owner, float delay) {
        Owner = owner;
        AddGoal(int.MaxValue, new IdleGoal(this));
        CurrentThought = new IdleThought(this, 0.2f);
        Delay = delay;
    }
    
    public Brain (Pawn owner, Dto dto) {
        Owner = owner;
        CurrentThought = dto.CurrentThoughtDto.Map(t => ThoughtFactory.CreateThought(this, t));
        Goals = new(dto.Goals.Map(goal => new KeyValuePair<int, Goal>(goal.Key, GoalFactory.CreateGoal(goal.Value, this))));
        OrderedGoals = Goals.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        Delay = dto.Delay;
    }
    
    public Dictionary<int, Goal> Goals { get; } = new ();
    public List<Goal> OrderedGoals { get; private set; } = new ();
    public Option<Thought> CurrentThought { get; private set; }
    public Pawn Owner { get; }
    public float Delay { get; set; }

    public void AddGoal(int priority, Goal goal) {
        Goals.Add(priority, goal);
        OrderedGoals = Goals.OrderBy(x => x.Key).Select(x => x.Value).ToList();
    }
    
    public void AddGoal(int priority, GoalType goalType)
        => AddGoal(priority, GoalFactory.CreateGoal(goalType, this));
    
    public void RemoveGoal(Goal goal) {
        Goals.Remove(Goals.First(x => x.Value == goal).Key);
        OrderedGoals = Goals.OrderBy(x => x.Key).Select(x => x.Value).ToList();
    }
    
    public void FinishThought() {
        CurrentThought.IfSome(thought => thought.Dispose());
        var nextThought = OrderedGoals.Select(goal => goal.NextThought()).Somes().HeadOrNone();
        nextThought.IfSome(thought => { CurrentThought = thought; thought.Initialize(); });
    }

    protected void Dispose(bool disposing) {
        if (disposing) CurrentThought.IfSome(thought => thought.Dispose());
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    public Dto ToDto() => new() {
        CurrentThoughtDto = CurrentThought.Map(thought => thought.ToDto()), 
        Goals = new(Goals.Map(kvp => new KeyValuePair<int, Goal.Dto>(kvp.Key, kvp.Value.ToDto()))), 
        Delay = Delay,
    };
    
    private static KeyValuePair<int, Goal.Dto> OrderedGoalToDto(KeyValuePair<int, Goal> pair)
        => new (pair.Key, pair.Value.ToDto());
    
    private static KeyValuePair<int, Goal> OrderedDtoToGoal(KeyValuePair<int, Goal.Dto> pair, Brain brain)
        => new (pair.Key, GoalFactory.CreateGoal(pair.Value, brain));
}