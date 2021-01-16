using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Actors.GameObjects;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.MovementControllers;
using TankGame.Src.Data.Map;

namespace TankGame.Src.Actors.Pawns
{
    internal static class EnemyFactory
    {
        private static readonly List<Vector2i> MovementVectors = new List<Vector2i> { new Vector2i(1, 0), new Vector2i(-1, 0), new Vector2i(0, 1), new Vector2i(0, -1) };

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

        private static bool MoveSpawnIfNecessary(Enemy enemy, Region region)
        {
            if (region.GetFieldAtMapCoords(enemy.Coords)?.PawnOnField != null)
            {
                Vector2i movementVector = MovementVectors.FirstOrDefault(vector
                    => region.GetFieldAtMapCoords(enemy.Coords + vector).IsTraversible()
                       && region.GetFieldAtMapCoords(enemy.Coords + vector) != null
                       && region.GetFieldAtMapCoords(enemy.Coords + vector).PawnOnField == null
                       && (region.GetFieldAtMapCoords(enemy.Coords).GameObject == null
                           || region.GetFieldAtMapCoords(enemy.Coords).GameObject.IsDestructibleOrTraversible));

                if (movementVector != default)
                {
                    enemy.Coords += movementVector;
                    GameObject gameObject = region.GetFieldAtMapCoords(enemy.Coords).GameObject;
                    if (gameObject != null && gameObject.IsDestructible) gameObject.OnDestroy();
                    return true;
                }
                else return false;
            }
            else
            {
                GameObject gameObject = region.GetFieldAtMapCoords(enemy.Coords).GameObject;
                if (gameObject != null)
                {
                    if (gameObject.IsDestructibleOrTraversible)
                    {
                        if (gameObject.IsDestructible) gameObject.OnDestroy();
                        return true;
                    }
                    else return false;
                }
                else return true;
            }
        }

        public static Enemy CreateEnemy(Vector2i coords, string enemyType, string AIMCType, List<Vector2i> patrolRoute, int health, Region region)
        {
            return null;

            if (EnemyTypes.ContainsKey(enemyType) && AIMCTypes.ContainsKey(AIMCType))
            {
                Enemy newEnemy = EnemyTypes[enemyType](new Vector2f(coords.X, coords.Y));
                if (health != -1) newEnemy.Health = health;

                newEnemy.MovementController = AIMCTypes[AIMCType](newEnemy switch { LightTank _ => 0.75, MediumTank _ => 1.5, HeavyTank _ => 2.25, _ => 1 }, newEnemy, patrolRoute);

                if (MoveSpawnIfNecessary(newEnemy, region))
                {
                    region.GetFieldAtMapCoords(newEnemy.Coords).PawnOnField = newEnemy;
                    return newEnemy;
                }
                else
                {
                    newEnemy.OnDestroy();
                    return null;
                }
            }
            else return null;
        }

        public static Enemy CreateEnemy(EnemySpawnData enemySpawnData, int health, Region region) => CreateEnemy(enemySpawnData.Coords, enemySpawnData.Type, enemySpawnData.AimcType, enemySpawnData.PatrolRoute, health, region);
    }
}