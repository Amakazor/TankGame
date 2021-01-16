using SFML.System;
using System.Collections.Generic;

namespace TankGame.Src.Actors.Pawns.Enemies
{
    internal struct EnemySpawnData
    {
        public Vector2i Coords { get; }
        public string Type { get; }
        public string AimcType { get; }
        public List<Vector2i> PatrolRoute { get; }

        public EnemySpawnData(Vector2i coords, string type, string aimcType, List<Vector2i> patrolRoute = null)
        {
            Coords = coords;
            Type = type;
            AimcType = aimcType;
            PatrolRoute = patrolRoute;
        }
    }
}