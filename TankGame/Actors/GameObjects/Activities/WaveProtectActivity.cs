using System.Collections.Generic;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Actors.Pawns.Enemies;

namespace TankGame.Actors.GameObjects.Activities;

public class WaveProtectActivity : WaveActivity {
    [JsonConstructor] public WaveProtectActivity(Vector2i coords, string name, string type, int? health, int pointsAdded, Queue<List<EnemySpawnData>> enemySpawns, ActivityStatus activityStatus, int? enemiesCount, uint currentWave = 0) : base(coords, name, type, health ?? enemySpawns.Count, pointsAdded, enemySpawns, activityStatus, enemiesCount, currentWave) { }
}