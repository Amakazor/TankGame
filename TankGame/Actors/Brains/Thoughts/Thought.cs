using System;
using System.Text.Json.Serialization;
using TankGame.Core;
using TankGame.Core.Sounds;

namespace TankGame.Actors.Brains.Thoughts; 



public abstract class Thought : ITickable, IDisposable {

    [JsonDerivedType(typeof(RotateThought.Dto), typeDiscriminator: nameof(RotateThought))]
    public class Dto {
        public float TotalTime { get; set; }
        public float TimeLeft { get; set; }
        
        public void Deconstruct(out float totalTime, out float timeLeft) {
            totalTime = TotalTime;
            timeLeft = TimeLeft;
        }
    }
    
    public Thought(Brain brain, Dto dto) {
        Brain = brain;
        (TotalTime, TimeLeft) = dto;
    }
    
    public Thought(Brain brain, float totalTime) {
        Brain = brain;
        // TotalTime = totalTime;
        TotalTime = 0.2f;
        TimeLeft = TotalTime;
        
        (this as ITickable).RegisterTickable();
    }

    public Brain Brain { get; }
    public float TotalTime { get; }
    public float TimeLeft { get; protected set; }

    public float Completion => 1 - TimeLeft / TotalTime;

    public virtual void Tick(float deltaTime, bool checkForCompletion = true) {
        TimeLeft = Math.Max(0, TimeLeft - deltaTime);
        if (checkForCompletion) CheckForCompletion();
    }
    
    public virtual void Tick(float deltaTime) {
        TimeLeft = Math.Max(0, TimeLeft - deltaTime);
        CheckForCompletion();
    }

    public abstract void Initialize();

    public abstract void FinishThought();
    
    protected void CheckForCompletion() {
        if (TimeLeft > 0) return;
        FinishThought();
        Brain.FinishThought();
    }
    
    protected virtual void Dispose(bool disposing) {
        if (disposing) (this as ITickable).UnregisterTickable();
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}