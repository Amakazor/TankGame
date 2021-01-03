﻿using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Actors.Pawns.Enemies;

namespace TankGame.Src.Actors.GameObjects.Activities
{
    internal class ProtectActivity : DestroyAllActivity
    {
        public ProtectActivity(Vector2i coords, HashSet<Enemy> enemies, int? hp = null) : base(coords, enemies, hp ?? 3, "Protect the tower. Destroy all enemies", 2000, new Tuple<TraversibilityData, DestructabilityData>(new TraversibilityData(1, false), new DestructabilityData(6, true, false)))
        {
        }

        public override void OnDestroy()
        {
            ChangeStatus(ActivityStatus.Failed);
            base.OnDestroy();
        }
    }
}