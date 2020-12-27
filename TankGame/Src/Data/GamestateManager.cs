using System;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Data.Map;

namespace TankGame.Src.Data
{
    internal class GamestateManager
    {
        private static GamestateManager instance;

        public long Points { get; private set; }
        public GameMap Map { get; set; }
        public Player Player { get; set; }
        public Random Random { get; }
        public static GamestateManager Instance => instance ?? (instance = new GamestateManager());

        private GamestateManager()
        {
            Points = 0;
            Random = new Random();
        }

        public void AddPoints(long points) => Points += points;
    }
}