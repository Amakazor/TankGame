using System.Collections.Generic;
using System.Text.Json.Serialization;
using SFML.System;

namespace TankGame.Actors.Pawns.Enemies;

public struct EnemySpawnData {
    public Vector2i Coords { get; set; }
    public EnemyType Type { get; }
    public List<Vector2i>? PatrolRoute { get; }

    [JsonConstructor] public EnemySpawnData(Vector2i coords, EnemyType type, List<Vector2i>? patrolRoute = null) {
        Coords = coords;
        Type = type;
        PatrolRoute = patrolRoute;
    }
}