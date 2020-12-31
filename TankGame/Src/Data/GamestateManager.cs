using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Actors.Text;
using TankGame.Src.Data.Map;

namespace TankGame.Src.Data
{
    internal class GamestateManager
    {
        private const double ComboTime = 5;
        private const uint MaxCombo = 10;

        private static GamestateManager instance;

        public ulong Points { get; private set; }
        private uint Combo { get; set; }
        private double ComboDeltaTimeCummulated { get; set; }
        public GameMap Map { get; set; }
        public Player Player { get; set; }
        public Random Random { get; }
        public static GamestateManager Instance => instance ?? (instance = new GamestateManager());
        private HashSet<PointsAddedTextBox> PointsTextBoxes;

        private GamestateManager()
        {
            Points = 0;
            Combo = 1;
            Random = new Random();
            PointsTextBoxes = new HashSet<PointsAddedTextBox>();
        }

        public void AddPoints(uint points, Vector2f? position = null)
        {
            ComboDeltaTimeCummulated = 0;
            Points += points * Combo;
            PointsTextBoxes.Add(new PointsAddedTextBox(position ?? Player.Position + new Vector2f((Player.Size.X / 2) - 50, (Player.Size.Y / 4) - 10), points, Combo));
            Combo = Math.Min(Combo + 1, MaxCombo);
        }

        public void Tick(float deltaTime)
        {
            foreach (PointsAddedTextBox pointsAddedTextBox in PointsTextBoxes.ToList()) if (pointsAddedTextBox.TimeToLive < 0) PointsTextBoxes.Remove(pointsAddedTextBox);

            ComboDeltaTimeCummulated += deltaTime;

            if (ComboDeltaTimeCummulated > ComboTime)
            {
                ComboDeltaTimeCummulated = 0;
                Combo = Math.Max(1, Combo - 1);
            }
        }
    }
}