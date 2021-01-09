using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Data.Map;

namespace TankGame.Src.Actors.GameObjects.Activities
{
    internal class WaveProtectActivity : WaveActivity
    {
        public WaveProtectActivity(Vector2i coords, HashSet<Enemy> enemies, Queue<List<EnemySpawnData>> enemySpawns, Region region, uint currentWave = 0, int? hp = null, string name = null, int? pointsAdded = null, Tuple<TraversibilityData, DestructabilityData, string> gameObjectType = null) : base(coords, enemies, enemySpawns, region, currentWave, hp != -1 ? hp : 5 * enemySpawns.Count(), name ?? "Destroy all enemies", "waveprotect", pointsAdded ?? 5000, gameObjectType ?? new Tuple<TraversibilityData, DestructabilityData, string>(new TraversibilityData(1, false), new DestructabilityData(3, true, false), "towerdestroyed"))
        {
        }

        public override void OnDestroy()
        {
            ChangeStatus(ActivityStatus.Failed);
            base.OnDestroy();
        }
    }
}
