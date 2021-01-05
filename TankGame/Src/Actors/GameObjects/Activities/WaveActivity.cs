using SFML.System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Data;

namespace TankGame.Src.Actors.GameObjects.Activities
{
    class WaveActivity : Activity
    {
        protected Queue<List<EnemySpawnData>> EnemySpawns { get; }
        protected uint CurrentWave { get; set; }

        public WaveActivity(Vector2i coords, HashSet<Enemy> enemies, Queue<List<EnemySpawnData>> enemySpawns, uint currentWave = 0, int? hp = null, string name = null, int? pointsAdded = null, Tuple<TraversibilityData, DestructabilityData, string> gameObjectType = null) : base(coords, enemies, hp ?? 1, name ?? "Destroy all enemies", gameObjectType ?? new Tuple<TraversibilityData, DestructabilityData, string>(new TraversibilityData(1, false), new DestructabilityData(1, false, false), null), pointsAdded ?? 3000)        
        {
            EnemySpawns = enemySpawns;
            CurrentWave = currentWave;
            AllEnemiesCount = (uint)Enemies.Count;
            if (AllEnemiesCount == 0 && (enemySpawns == null || EnemySpawns.Count == 0)) ActivityStatus = ActivityStatus.Completed;

            PointsAdded *= (int)currentWave + (enemySpawns == null ? 0 : enemySpawns.Count);
        }


        protected override string CalculateProgress()
        {
            if (Enemies.Count == 0 && ActivityStatus == ActivityStatus.Started)
            {
                if (EnemySpawns.Count == 0) ChangeStatus(ActivityStatus.Completed);
                else SpawnNextWave();
            }

            if (ActivityStatus == ActivityStatus.Completed || ActivityStatus == ActivityStatus.Failed) return "";

            return "Enemy " + (AllEnemiesCount - Enemies.Count) + " of " + AllEnemiesCount + "\n" +
                   "Wave " + CurrentWave + " of " + (CurrentWave + (EnemySpawns != null ? EnemySpawns.Count : 0));
        }

        public override void ChangeStatus(ActivityStatus activityStatus)
        {
            base.ChangeStatus(activityStatus);
            if (ActivityStatus == ActivityStatus.Started) SpawnNextWave();
        }

        protected void SpawnNextWave()
        {
            CurrentWave++;
            new List<Enemy>(from enemySpawnData in EnemySpawns.Dequeue() select EnemyFactory.CreateEnemy(enemySpawnData, 0)).ForEach(enemy => { CurrentRegion.GetFieldAtMapCoords(enemy.Coords).PawnOnField = enemy; Enemies.Add(enemy); });
            AllEnemiesCount = (uint)Enemies.Count;
            if (AllEnemiesCount == 0 && EnemySpawns.Count == 0 && ActivityStatus != ActivityStatus.Completed) ChangeStatus(ActivityStatus.Completed);
        }
    }
}
