using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Actors.Data;
using TankGame.Src.Actors.Pawns.Enemies;

namespace TankGame.Src.Actors.GameObjects.Activities
{
    internal class ProtectActivity : DestroyAllActivity
    {
        public ProtectActivity(Vector2i coords, HashSet<Enemy> enemies, int? hp = null) : base(coords, enemies, hp != -1 ? hp : 15, "Protect the tower. Destroy all enemies", "protect", 2000, new Tuple<TraversibilityData, DestructabilityData, string>(new TraversibilityData(1, false), new DestructabilityData(6, true, false), "towerdestroyed"))
        {
            if (hp != null && hp == 0) ActivityStatus = ActivityStatus.Failed;
        }

        public override void OnDestroy()
        {
            ChangeStatus(ActivityStatus.Failed);
            base.OnDestroy();
        }
    }
}