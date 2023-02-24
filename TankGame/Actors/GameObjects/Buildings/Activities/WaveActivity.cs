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
        return "";
    }

    public override void ChangeStatus(ActivityStatus activityStatus) {
       //TODO: Add logic for changing status
    }

    protected void SpawnNextWave() {
        CurrentWave++;
        foreach (var enemy in EnemySpawns.Dequeue()) EnemyFactory.CreateEnemy(enemy);
        if (EnemiesCount == 0 && EnemySpawns.Count == 0 && ActivityStatus != ActivityStatus.Completed) ChangeStatus(ActivityStatus.Completed);
    }

    public override DestructabilityType DestructabilityType => DestructabilityType.Indestructible;
    public override Dto ToDto()
        => new() {Coords = Coords, Health = Health, EnemiesCount = EnemiesCount, Name = Name, PointsAdded = PointsAdded, ActivityStatus = ActivityStatus, EnemySpawns = EnemySpawns, CurrentWave = CurrentWave};
}