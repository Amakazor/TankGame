using System.Collections.Generic;
using LanguageExt;
using SFML.System;
using TankGame.Actors.GameObjects.Buildings.Utility;
using TankGame.Actors.Pawns.Enemies;
using TankGame.Core.Map;

namespace TankGame.Actors.GameObjects.Buildings.Activities;

public class WaveProtectActivity : WaveActivity {
     public new class Dto : WaveActivity.Dto { }
     public WaveProtectActivity(Vector2i coords, string name, int health, int pointsAdded, Queue<Seq<EnemySpawnData>> enemySpawns, ActivityStatus activityStatus, int enemiesCount, uint currentWave = 0) : base(coords, name, health, pointsAdded, enemySpawns, activityStatus, enemiesCount, currentWave) { }
     
     public WaveProtectActivity(Dto dto, Region region) : base(dto, region) { }
     
     public override DestructabilityType DestructabilityType => DestructabilityType.Indestructible;
}