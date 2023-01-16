using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Actors.Brains.Goals;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.Pawns;

namespace TankGame.Actors.Brains; 

public class Brain : IDisposable {
    public Brain(Pawn owner) {
        Owner = owner;
        AddGoal(int.MaxValue, new IdleGoal(this));
        CurrentThought = new IdleThought(this, 0.2f);
    }
    
    public Dictionary<int, Goal> Goals { get; } = new ();
    public List<Goal> OrderedGoals { get; private set; } = new ();
    public Thought? CurrentThought { get; private set; }
    public Pawn Owner { get; }
    public float Delay { get; set; }

    public void AddGoal(int priority, Goal goal) {
        Goals.Add(priority, goal);
        OrderedGoals = Goals.OrderBy(x => x.Key).Select(x => x.Value).ToList();
    }
    
    public void RemoveGoal(Goal goal) {
        Goals.Remove(Goals.First(x => x.Value == goal).Key);
        OrderedGoals = Goals.OrderBy(x => x.Key).Select(x => x.Value).ToList();
    }
    
    public void FinishThought() {
        CurrentThought?.Dispose();

        foreach (Thought? thought in OrderedGoals.Select(goal => goal.NextThought()).Where(thought => thought != null)) {
            CurrentThought = thought;
            CurrentThought?.Initialize();
            break;
        }
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            CurrentThought?.Dispose();
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}