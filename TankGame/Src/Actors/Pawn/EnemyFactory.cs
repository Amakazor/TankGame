using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Actors.Pawn.Enemies;
using TankGame.Src.Actors.Pawn.MovementControllers;

namespace TankGame.Src.Actors.Pawn
{
    internal static class EnemyFactory
    {
        private static Dictionary<string, Func<float, Enemy, AIMovementController>> AIMCTypes = new Dictionary<string, Func<float, Enemy, AIMovementController>>
        {
            {"random", (delay, owner) =>  new RandomAIMovementController(delay, owner) }
        };

        private static Dictionary<string, Func<Vector2f, Enemy>> EnemyTypes = new Dictionary<string, Func<Vector2f, Enemy>>
        {
            { "light", (Coords) => new LightTank(Coords*64, new Vector2f(64, 64)) },
            { "medium", (Coords) => new MediumTank(Coords*64, new Vector2f(64, 64)) },
            { "heavy", (Coords) => new HeavyTank(Coords*64, new Vector2f(64, 64)) }
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
            Enemy newEnemy = CreateEnemy(coords, enemyType, AIMCType);
            newEnemy.HP = Hp;
            return newEnemy;
        }
    }
}
