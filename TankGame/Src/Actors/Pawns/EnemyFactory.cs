using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.MovementControllers;
using TankGame.Src.Data.Map;
using TankGame.Src.Extensions;

namespace TankGame.Src.Actors.Pawns
{
    internal static class EnemyFactory
    {
        private static readonly Dictionary<string, Func<double, Enemy, List<Vector2i>, AIMovementController>> AIMCTypes = new Dictionary<string, Func<double, Enemy, List<Vector2i>, AIMovementController>>
        {
            {"random", (delay, owner, patrolRoute) =>  new RandomAIMovementController     (delay, owner)},
            {"chase",  (delay, owner, patrolRoute) =>  new ChaseAIMovementController      (delay, owner)},
            {"stand",  (delay, owner, patrolRoute) =>  new StandGroundAIMovementController(delay, owner)},
            {"patrol", (delay, owner, patrolRoute) =>  new PatrolAIMovementController     (delay, owner, patrolRoute)}
        };

        private static readonly Dictionary<string, Func<Vector2f, Enemy>> EnemyTypes = new Dictionary<string, Func<Vector2f, Enemy>>
        {
            { "light",  (Coords) => new LightTank (Coords*64, new Vector2f(64, 64))},
            { "medium", (Coords) => new MediumTank(Coords*64, new Vector2f(64, 64))},
            { "heavy",  (Coords) => new HeavyTank (Coords*64, new Vector2f(64, 64))}
        };

        public static Enemy CreateEnemy(Vector2i coords, string enemyType, string AIMCType, List<Vector2i> patrolRoute, int health, Region region)
        {
            if (EnemyTypes.ContainsKey(enemyType) && AIMCTypes.ContainsKey(AIMCType))
            {
                Enemy newEnemy = EnemyTypes[enemyType](new Vector2f(coords.X, coords.Y));
                if (health != -1) newEnemy.Health = health;
                
                newEnemy.MovementController = AIMCTypes[AIMCType](newEnemy switch{LightTank _ => 0.75, MediumTank _ => 1.5, HeavyTank _ => 2.25, _ => 1}, newEnemy, patrolRoute);

                if (region.GetFieldAtMapCoords(newEnemy.Coords).PawnOnField != null)
                {
                    if (!new List<Vector2i> { new Vector2i(1, 0), new Vector2i(-1, 0), new Vector2i(0, 1), new Vector2i(0, -1) }.Any(vector => ((region.GetFieldAtMapCoords(newEnemy.Coords + vector).IsTraversible() && region.GetFieldAtMapCoords(newEnemy.Coords + vector) != null && region.GetFieldAtMapCoords(newEnemy.Coords + vector).PawnOnField == null) ? newEnemy.Coords += vector : new Vector2i(-1, -1)).IsValid()))
                    {
                        newEnemy.OnDestroy();
                        return null;
                    }
                }

                if (region.GetFieldAtMapCoords(newEnemy.Coords).GameObject != null)
                {
                    if (region.GetFieldAtMapCoords(newEnemy.Coords).GameObject.IsDestructibleOrTraversible && region.GetFieldAtMapCoords(newEnemy.Coords).GameObject.IsDestructible) region.GetFieldAtMapCoords(newEnemy.Coords).GameObject.OnDestroy();
                    else
                    {
                        newEnemy.OnDestroy();
                        return null;
                    }
                }
                
                region.GetFieldAtMapCoords(newEnemy.Coords).PawnOnField = newEnemy;
                return newEnemy;
            }
            else throw new Exception();
        }

        public static Enemy CreateEnemy(EnemySpawnData enemySpawnData, int health, Region region) => CreateEnemy(enemySpawnData.Coords, enemySpawnData.Type, enemySpawnData.AimcType, enemySpawnData.PatrolRoute, health, region);
    }
}
