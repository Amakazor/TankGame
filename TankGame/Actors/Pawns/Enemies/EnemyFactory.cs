using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Brains.Goals;
using TankGame.Core.Gamestate;
using TankGame.Core.Map;
using TankGame.Core.Textures;

namespace TankGame.Actors.Pawns.Enemies;

public static class EnemyFactory {
    private static readonly List<Vector2i> MovementVectors = new() {
        new(1, 0), new(-1, 0), new(0, 1), new(0, -1),
    };

    private static readonly Dictionary<EnemyType, string> Textures = new() { { EnemyType.Light, "enemy1" }, { EnemyType.Medium, "enemy2" }, { EnemyType.Heavy, "enemy3" } };

    public static readonly Dictionary<EnemyType, int> DefaultHealth = new() { { EnemyType.Light, 1 }, { EnemyType.Medium, 2 }, { EnemyType.Heavy, 3 } };

    public static readonly Dictionary<EnemyType, double> MovementDelays = new() { { EnemyType.Light, 0.75 }, { EnemyType.Medium, 1.50 }, { EnemyType.Heavy, 2.25 } };

    public static Texture GetTexture(EnemyType enemyType)
        => TextureManager.Get(TextureType.Pawn, Textures[enemyType]);

    private static int GetHealth(EnemyType enemyType, int health)
        => health == -1 ? DefaultHealth[enemyType] : health;

    private static bool NeedsToMoveSpawn(Vector2i coords, Region region)
        => region.GetFieldAtMapCoords(coords)
                ?.PawnOnField is not null;

    private static (bool canMoveCoords, Vector2i movedCoords) GetSpawnMovementData(Vector2i coords, Region region) {
        Vector2i movementVector = MovementVectors.FirstOrDefault(
            vector => region.GetFieldAtMapCoords(coords + vector)
                           ?.CanBeSpawnedOn() ?? false
        );
        return (movementVector != default, movementVector + coords);
    }

    public static Option<Enemy> CreateEnemy(EnemySpawnData enemySpawnData, int health = -1) {
        return GamestateManager.Map.GetRegionFromFieldCoords(enemySpawnData.Coords).Match<Option<Enemy>>(
            region => {
                if (NeedsToMoveSpawn(enemySpawnData.Coords, region)) {
                    (bool canMoveCoords, Vector2i movedCoords) = GetSpawnMovementData(enemySpawnData.Coords, region);

                    if (!canMoveCoords) return None;
                    enemySpawnData.Coords = movedCoords;
                }

                Vector2f scaledCoords = new(enemySpawnData.Coords.X * 64.0f, enemySpawnData.Coords.Y * 64.0f);
                Vector2f size = new(64.0f, 64.0f);
                int score = ((int)enemySpawnData.Type + 1) * 100;
                Enemy enemy = new(scaledCoords, size, GetTexture(enemySpawnData.Type), GetHealth(enemySpawnData.Type, health), score, enemySpawnData.Type);

                region.GetFieldAtMapCoords(enemy.Coords)!.PawnOnField = enemy;
                region.Enemies.Add(enemy);
                return enemy;
            }, None
        );
    }
}