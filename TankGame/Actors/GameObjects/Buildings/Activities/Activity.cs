using System;
using System.Text.Json.Serialization;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.GameObjects.Buildings.Towers;
using TankGame.Actors.GameObjects.Buildings.Utility;
using TankGame.Core.Gamestates;
using TankGame.Core.Map;
using TankGame.Core.Textures;

namespace TankGame.Actors.GameObjects.Buildings.Activities;

public abstract class Activity : GameObject, ITickable {

    [JsonDerivedType(typeof(WaveActivity.Dto),        nameof(WaveActivity))]
    [JsonDerivedType(typeof(WaveProtectActivity.Dto), nameof(WaveProtectActivity))]
    [JsonDerivedType(typeof(DestroyAllActivity.Dto),  nameof(DestroyAllActivity))]
    [JsonDerivedType(typeof(ProtectActivity.Dto),     nameof(ProtectActivity))]
    public new class Dto : GameObject.Dto {
        [JsonIgnore] public Option<int> EnemiesCountOption {
            get => Optional(EnemiesCount);
            set => EnemiesCount = value.Match<int?>(
                Some: i => i,
                None: () => null
            );
        }
        
        public Vector2i Coords { get; set; }
        public int? EnemiesCount { get; set; }
        public string Name { get; set; }
        public int PointsAdded { get; set; }
        public ActivityStatus ActivityStatus { get; set; }
    }
    
    private static readonly Texture Tex = TextureManager.Get(TextureType.GameObject, "tower");
    public Activity(Vector2i coords, string name, int health, int pointsAdded, ActivityStatus activityStatus, int enemiesCount) : base(coords, Tex, health) {
        EnemiesCount = enemiesCount;
        Name = name;
        PointsAdded = pointsAdded;
        ActivityStatus = activityStatus;
        if (_health == 0) ActivityStatus = ActivityStatus.Failed;
        (this as ITickable).RegisterTickable();
    }

    public Activity(Dto dto, Region region) : base(dto, Tex, dto.Coords) {
        EnemiesCount = dto.EnemiesCount.IfNone(region.Enemies.Count);
        Name = dto.Name;
        PointsAdded = dto.PointsAdded;
        ActivityStatus = dto.ActivityStatus;
    }

    [JsonIgnore] public int EnemiesCount { get; set; }
    public string Name { get; init; }

    [JsonIgnore] public string DisplayName => ActivityStatus switch {
        ActivityStatus.Started   => Name,
        ActivityStatus.Completed => "Completed",
        ActivityStatus.Failed    => "Failed",
        _                        => "",
    };

    public string ProgressText => CalculateProgress();
    public int PointsAdded { get; }
    public ActivityStatus ActivityStatus { get; protected set; }

    public void Tick(float deltaTime)
        => CalculateProgress();

    public virtual void ChangeStatus(ActivityStatus activityStatus) {
        if (ActivityStatus == ActivityStatus.Failed || ActivityStatus == activityStatus) return;

        ActivityStatus = activityStatus;
        switch (activityStatus) {
            case ActivityStatus.Completed:
                Gamestate.CompleteActivity(PointsAdded, Position);
                ChangeToCompleted();
                break;
            case ActivityStatus.Failed:
                Gamestate.FailActivity(PointsAdded / 4, Position);
                break;
            case ActivityStatus.Stopped: break;
            case ActivityStatus.Started: break;
            default:                     throw new ArgumentOutOfRangeException(nameof(activityStatus), activityStatus, null);
        }

        // if (Gamestate.Level != null) Gamestate.Save();
    }

    protected void ChangeToCompleted() {
        Gamestate.Level.FieldAt(Coords).IfSome(field => field.GameObject = new CompletedTower(Coords));
        Dispose();
    }

    protected abstract string CalculateProgress();

    public override void Dispose() {
        GC.SuppressFinalize(this);
        (this as ITickable).UnregisterTickable();
        base.Dispose();
    }

    public override float SpeedModifier => 0;

    public override bool Traversible => false;

    protected override Option<GameObject> AfterDestruction => new CompletedTower(Coords);

    public override bool StopsProjectile => false;

    public override void Destroy() {
        ChangeStatus(ActivityStatus.Failed);
        base.Destroy();
    }

    public override Dto ToDto()
        => new() {Coords = Coords, Health = Health, EnemiesCount = EnemiesCount, Name = Name, PointsAdded = PointsAdded, ActivityStatus = ActivityStatus};
}