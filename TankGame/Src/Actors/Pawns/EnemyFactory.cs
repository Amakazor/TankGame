using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.MovementControllers;

namespace TankGame.Src.Actors.Pawns
{
    internal static class EnemyFactory
    {
        private static readonly Dictionary<string, Func<float, Enemy, AIMovementController>> AIMCTypes = new Dictionary<string, Func<float, Enemy, AIMovementController>>
        {
            {"random", (delay, owner) =>  new RandomAIMovementController(delay, owner)},
            {"chase",  (delay, owner) =>  new ChaseAIMovementController(delay, owner)},
            {"stand",  (delay, owner) =>  new StandGroundAIMovementController(delay, owner)}
        };

        private static readonly Dictionary<string, Func<Vector2f, Enemy>> EnemyTypes = new Dictionary<string, Func<Vector2f, Enemy>>
        {
            { "light",  (Coords) => new LightTank (Coords*64, new Vector2f(64, 64))},
            { "medium", (Coords) => new MediumTank(Coords*64, new Vector2f(64, 64))},
            { "heavy",  (Coords) => new HeavyTank (Coords*64, new Vector2f(64, 64))}
        };

        public static Enemy CreateEnemy(Vector2f coords, string enemyType, string AIMCType)
        {
            if (EnemyTypes.ContainsKey(enemyType) && AIMCTypes.ContainsKey(AIMCType))
            {
                Enemy newEnemy = EnemyTypes[enemyType](coords);

                newEnemy.MovementController = AIMCTypes[AIMCType](newEnemy switch{LightTank _ => 1, MediumTank _ => 2, HeavyTank _ => 3, _ => 1}, newEnemy);

                return newEnemy;
            }
            else throw new Exception();
        }

        public static Enemy CreateEnemy(Vector2f coords, string enemyType, string AIMCType, int Hp)
        {
            return Hp != 0 ? CreateEnemy(coords, enemyType, AIMCType, Hp) : CreateEnemy(coords, enemyType, AIMCType);
        }
    }
}
