using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Actors.Pawns.Enemies;

namespace TankGame.Src.Actors.GameObjects.Activities
{
    internal class WaveProtectActivity : WaveActivity
    {
        public WaveProtectActivity(Vector2i coords, HashSet<Enemy> enemies, Queue<List<EnemySpawnData>> enemySpawns, uint currentWave = 0, int? hp = null, string name = null, int? pointsAdded = null, Tuple<TraversibilityData, DestructabilityData> gameObjectType = null) : base(coords, enemies, enemySpawns, currentWave, hp ?? 3 * enemySpawns.Count(), name ?? "Destroy all enemies", pointsAdded ?? 5000, gameObjectType ?? new Tuple<TraversibilityData, DestructabilityData>(new TraversibilityData(1, false), new DestructabilityData(3, true, false)))
        {
        }

        public override void OnDestroy()
        {
            ChangeStatus(ActivityStatus.Failed);
            base.OnDestroy();
        }
    }
}
