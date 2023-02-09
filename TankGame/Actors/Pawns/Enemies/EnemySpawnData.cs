using System.Text.Json.Serialization;
using SFML.System;

namespace TankGame.Actors.Pawns.Enemies;

public struct EnemySpawnData {
    public Vector2i Coords { get; set; }
    public EnemyType Type { get; }

    [JsonConstructor] public EnemySpawnData(Vector2i coords, EnemyType type) {
        Coords = coords;
        Type = type;
    }
}