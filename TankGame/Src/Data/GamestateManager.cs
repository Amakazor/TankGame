using System;
using TankGame.Src.Actors.Pawn.Player;
using TankGame.Src.Data.Map;

namespace TankGame.Src.Data
{
    internal class GamestateManager
    {
        private static GamestateManager instance;

        public long Points { get; private set; }
        public GameMap Map { private get; set; }
        public Player Player { get; set; }
        public static GamestateManager Instance { get { return instance ?? (instance = new GamestateManager()); } }

        public void AddPoints(long points)
        {
            Points += points;
        }

        public GameMap GetMap()
        {
            if (Map != null)
            {
                return Map;
            }
            else throw new Exception("Map is null");
        }

        private GamestateManager()
        {
            Points = 0;
        }
    }
}