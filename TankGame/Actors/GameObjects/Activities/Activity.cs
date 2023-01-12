using System;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Core.Gamestate;

namespace TankGame.Actors.GameObjects.Activities;

[JsonDerivedType(typeof(WaveActivity), "Wave")]
[JsonDerivedType(typeof(WaveProtectActivity), "WaveProtect")]
[JsonDerivedType(typeof(DestroyAllActivity), "Destroy")]
[JsonDerivedType(typeof(ProtectActivity), "Protect")]
public abstract class Activity : GameObject, ITickable {
    [JsonConstructor] public Activity(Vector2i coords, string name, string type, int? health, int pointsAdded, ActivityStatus activityStatus, int? enemiesCount) : base(coords, type, health) {
        EnemiesCount = enemiesCount;
        Name = name;
        PointsAdded = pointsAdded;
        ActivityStatus = activityStatus;
        if (GameObjectType.DestructabilityData.Health is 0) ActivityStatus = ActivityStatus.Failed;
        (this as ITickable).RegisterTickable();
    }

    public int? EnemiesCount {
        get => AllEnemiesCount;
        init => AllEnemiesCount = value ?? 0;
    }

    [JsonIgnore] public int AllEnemiesCount { get; set; }
    public string Name { get; set; }

    [JsonIgnore] public string DisplayName {
        get => ActivityStatus switch {
            ActivityStatus.Started   => Name,
            ActivityStatus.Completed => "Completed",
            ActivityStatus.Failed    => "Failed",
            _                        => "",
        };

        private init => Name = value;
    }

    [JsonIgnore] public string ProgressText => CalculateProgress();
    public int PointsAdded { get; protected set; }
    public ActivityStatus ActivityStatus { get; protected set; }

    public void Tick(float deltaTime)
        => CalculateProgress();

    public virtual void ChangeStatus(ActivityStatus activityStatus) {
        if (ActivityStatus == ActivityStatus.Failed || ActivityStatus == activityStatus) return;

        ActivityStatus = activityStatus;
        switch (activityStatus) {
            case ActivityStatus.Completed:
                GamestateManager.CompleteActivity(PointsAdded, Position);
                ChangeToCompleted();
                break;
            case ActivityStatus.Failed:
                GamestateManager.FailActivity(PointsAdded / 4, Position);
                break;
            case ActivityStatus.Stopped: break;
            case ActivityStatus.Started: break;
            default:                     throw new ArgumentOutOfRangeException(nameof(activityStatus), activityStatus, null);
        }

        if (GamestateManager.Map != null) GamestateManager.Save();
    }

    protected void ChangeToCompleted() {
        GameObjectType = GameObjectTypes.TowerCompleted;
        GenerateSprite();
    }

    protected abstract string CalculateProgress();

    public override void Dispose() {
        GC.SuppressFinalize(this);
        (this as ITickable).UnregisterTickable();
        base.Dispose();
    }

    public override void OnDestroy() {
        ChangeStatus(ActivityStatus.Failed);
        base.OnDestroy();
    }
}