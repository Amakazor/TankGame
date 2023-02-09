using System.Collections.Generic;
using LanguageExt;
using SFML.System;
using TankGame.Actors.GameObjects.Buildings.Utility;
using TankGame.Actors.Pawns.Enemies;
using TankGame.Core.Map;

namespace TankGame.Actors.GameObjects.Buildings.Activities;

public class WaveActivity : Activity {
    public new class Dto : Activity.Dto {
        public Queue<Seq<EnemySpawnData>> EnemySpawns { get; set; }
        public uint CurrentWave { get; set; }
    }
    public WaveActivity(Vector2i coords, string name, int health, int pointsAdded, Queue<Seq<EnemySpawnData>> enemySpawns, ActivityStatus activityStatus, int enemiesCount, uint currentWave = 0) : base(coords, name, health, pointsAdded, activityStatus, enemiesCount) {
        EnemySpawns = enemySpawns;
        CurrentWave = currentWave;

        if (ActivityStatus == ActivityStatus.Failed || (EnemySpawns.Count != 0) || EnemiesCount != 0) return;
        ActivityStatus = ActivityStatus.Completed;
        ChangeToCompleted();
    }

    public WaveActivity(Dto dto, Region region) : base(dto, region) {
        EnemySpawns = dto.EnemySpawns;
        CurrentWave = dto.CurrentWave;

        if (ActivityStatus == ActivityStatus.Failed || (EnemySpawns.Count != 0) || EnemiesCount != 0) return;
        ActivityStatus = ActivityStatus.Completed;
        ChangeToCompleted();
    }


    public Queue<Seq<EnemySpawnData>> EnemySpawns { get; set; }
    public uint CurrentWave { get; set; }

    protected override string CalculateProgress() {
        // if (AllEnemiesCount == 0 && CurrentRegion.Enemies.Count > 0) AllEnemiesCount = CurrentRegion.Enemies.Count;
        //
        // if (CurrentRegion.Enemies.Count == 0 && ActivityStatus == ActivityStatus.Started) {
        //     if (EnemySpawns.Count == 0)
        //         ChangeStatus(ActivityStatus.Completed);
        //     else
        //         SpawnNextWave();
        // }
        //
        // if (ActivityStatus is ActivityStatus.Completed or ActivityStatus.Failed) return "";
        //
        // return "Enemy " + (AllEnemiesCount - CurrentRegion.Enemies.Count) + " of " + AllEnemiesCount + "\n" + "Wave  " + CurrentWave + " of " + (CurrentWave + (EnemySpawns?.Count ?? 0));

        return "";
    }

    public override void ChangeStatus(ActivityStatus activityStatus) {
        // base.ChangeStatus(activityStatus);
        // if (ActivityStatus == ActivityStatus.Started && CurrentRegion.Enemies.Count == 0 && EnemySpawns != null && EnemySpawns.Count != 0) SpawnNextWave();
    }

    protected void SpawnNextWave() {
        CurrentWave++;
        var _ = EnemySpawns.Dequeue().Select(EnemyFactory.CreateEnemy);
        if (EnemiesCount == 0 && EnemySpawns.Count == 0 && ActivityStatus != ActivityStatus.Completed) ChangeStatus(ActivityStatus.Completed);
    }

    public override DestructabilityType DestructabilityType => DestructabilityType.Indestructible;
    public override Dto ToDto()
        => new() {Coords = Coords, Health = Health, EnemiesCount = EnemiesCount, Name = Name, PointsAdded = PointsAdded, ActivityStatus = ActivityStatus, EnemySpawns = EnemySpawns, CurrentWave = CurrentWave};
}