using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Data;

namespace TankGame.Src.Actors.GameObjects.Activities
{
    internal class DestroyAllActivity : Activity
    {
        public DestroyAllActivity(Vector2i coords, HashSet<Enemy> enemies, int? hp = null, string name = null, int? pointsAdded = null, Tuple<TraversibilityData, DestructabilityData, string> gameObjectType = null) : base(coords, enemies, hp ?? 1, name??"Destroy all enemies", gameObjectType??new Tuple<TraversibilityData, DestructabilityData, string>(new TraversibilityData(1, false), new DestructabilityData(1, false, false), null), pointsAdded??1000)
        {
            AllEnemiesCount = (uint)Enemies.Count;
            if (AllEnemiesCount == 0) ActivityStatus = ActivityStatus.Completed;
        }

        protected override string CalculateProgress()
        {
            if (Enemies.Count == 0 && ActivityStatus != ActivityStatus.Completed) ChangeStatus(ActivityStatus.Completed);
            return (AllEnemiesCount - Enemies.Count) + " of " + AllEnemiesCount;
        }
    }
}
