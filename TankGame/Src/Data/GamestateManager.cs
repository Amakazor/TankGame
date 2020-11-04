namespace TankGame.Src.Data
{
    internal class GamestateManager
    {
        private static GamestateManager instance;

        public long Points { get; private set; }

        private GamestateManager()
        {
            Points = 0;
        }

        public static GamestateManager Instance { get { return instance ?? (instance = new GamestateManager()); } }

        public void AddPoints(long points)
        {
            Points += points;
        }
    }
}