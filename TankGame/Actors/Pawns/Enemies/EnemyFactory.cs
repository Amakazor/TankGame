using System;
using LanguageExt;
using SFML.System;
using TankGame.Actors.Fields;
using TankGame.Core.Gamestates;
using TankGame.Core.Map;

namespace TankGame.Actors.Pawns.Enemies;

public static class EnemyFactory {
    private static readonly Seq<Vector2i> MovementVectors = Seq<Vector2i>(new(1, 0), new(-1, 0), new(0, 1), new(0, -1));

    private static Option<Vector2i> GetSpawnMovementData(Field field)
        => MovementVectors
          .Filter(_ => field.CanBeSpawnedOn())
          .HeadOrNone();
    
    public static Option<Enemy> CreateEnemy(EnemySpawnData enemySpawnData)
        => MoveSpawnCoords(enemySpawnData.Coords)
           .Map<Enemy>(coords => enemySpawnData.Type switch { 
                EnemyType.Light  => new LightTank(coords),
                EnemyType.Medium => new MediumTank(coords),
                EnemyType.Heavy  => new HeavyTank(coords),
                _                => throw new ArgumentOutOfRangeException(), 
            });

    public static Option<Enemy> CreateEnemy(Enemy.Dto enemyDto, Region region)
        => MoveSpawnCoords(enemyDto.Coords, region)
          .Map(enemyDto.WithCoords)
          .Map<Enemy>(pawnDto => pawnDto switch {
               LightTank.Dto dto  => new LightTank(dto, region),
               MediumTank.Dto dto => new MediumTank(dto, region),
               HeavyTank.Dto dto  => new HeavyTank(dto, region),
               _                  => throw new ArgumentOutOfRangeException(),
           });

    private static Option<Vector2i> MoveSpawnCoords(Vector2i coords, Region region)
        => region.FieldAt(coords)
                 .Map(field => field.Pawn.IsSome ? GetSpawnMovementData(field) : coords)
                 .Flatten();
    
    private static Option<Vector2i> MoveSpawnCoords(Vector2i coords)
        => Gamestate.Level.FieldAt(coords)
                    .Map(field => field.Pawn.IsSome ? GetSpawnMovementData(field) : coords)
                    .Flatten();
}