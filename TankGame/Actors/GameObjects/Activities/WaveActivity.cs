using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Actors.Pawns.Enemies;

namespace TankGame.Actors.GameObjects.Activities;

public class WaveActivity : Activity {
    [JsonConstructor] public WaveActivity(Vector2i coords, string name, string type, int? health, int pointsAdded, Queue<List<EnemySpawnData>> enemySpawns, ActivityStatus activityStatus, int? enemiesCount, uint currentWave = 0) : base(coords, name, type, health, pointsAdded, activityStatus, enemiesCount)
        => SetBaseData(enemySpawns, currentWave);

    public Queue<List<EnemySpawnData>> EnemySpawns { get; set; }
    public uint CurrentWave { get; set; }

    private void SetBaseData(Queue<List<EnemySpawnData>> enemySpawns, uint currentWave) {
        EnemySpawns = enemySpawns;
        CurrentWave = currentWave;

        if (ActivityStatus == ActivityStatus.Failed || (enemySpawns != null && EnemySpawns.Count != 0) || AllEnemiesCount != 0) return;
        ActivityStatus = ActivityStatus.Completed;
        ChangeToCompleted();
    }

    protected override string CalculateProgress() {
        if (AllEnemiesCount == 0 && CurrentRegion.Enemies.Count > 0) AllEnemiesCount = CurrentRegion.Enemies.Count;

        if (CurrentRegion.Enemies.Count == 0 && ActivityStatus == ActivityStatus.Started) {
            if (EnemySpawns.Count == 0)
                ChangeStatus(ActivityStatus.Completed);
            else
                SpawnNextWave();
        }

        if (ActivityStatus is ActivityStatus.Completed or ActivityStatus.Failed) return "";

        return "Enemy " + (AllEnemiesCount - CurrentRegion.Enemies.Count) + " of " + AllEnemiesCount + "\n" + "Wave  " + CurrentWave + " of " + (CurrentWave + (EnemySpawns?.Count ?? 0));
    }

    public override void ChangeStatus(ActivityStatus activityStatus) {
        base.ChangeStatus(activityStatus);
        if (ActivityStatus == ActivityStatus.Started && CurrentRegion.Enemies.Count == 0 && EnemySpawns != null && EnemySpawns.Count != 0) SpawnNextWave();
    }

    protected void SpawnNextWave() {
        CurrentWave++;
        EnemySpawns.Dequeue()
                   .Select(enemySpawnData => EnemyFactory.CreateEnemy(enemySpawnData, -1, CurrentRegion))
                   .ToList()
                   .ForEach(enemy => { CurrentRegion.Enemies.Add(enemy); });

        if (AllEnemiesCount == 0 && EnemySpawns.Count == 0 && ActivityStatus != ActivityStatus.Completed) ChangeStatus(ActivityStatus.Completed);
    }
}