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
        private HashSet<Enemy> Enemies { get; set; }
        private readonly uint InitialEnemiesCount;

        public DestroyAllActivity(Vector2i coords, uint? initialEnemiesCount, HashSet<Enemy> enemies) : base(coords, new Tuple<TraversibilityData, DestructabilityData>(new TraversibilityData(1, false), new DestructabilityData(1, false, false)), TextureManager.Instance.GetTexture(TextureType.Activity, "tower"), "", 1, "Destroy all enemies", 1000)
        {
            Enemies = new HashSet<Enemy>(enemies);
            InitialEnemiesCount = initialEnemiesCount?? (uint)Enemies.Count;
            if (InitialEnemiesCount == 0) ActivityStatus = ActivityStatus.Completed; 
        }

        protected override string CalculateProgress()
        {
            if (Enemies.Count == 0 && ActivityStatus != ActivityStatus.Completed) ChangeStatus(ActivityStatus.Completed);
            return (InitialEnemiesCount - Enemies.Count) + " of " + InitialEnemiesCount;
        }

        public void OnEnemyDestruction(Enemy enemy) => Enemies.Remove(enemy);
    }
}
