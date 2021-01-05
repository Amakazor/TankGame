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
        private const double ComboTime = 7.5;
        private const uint MaxCombo = 10;

        private static GamestateManager instance;

        public long Points { get; private set; }
        private long PointsBeforeSubstraction { get; set; }
        private uint Combo { get; set; }
        private int CompletedActivities { get; set; }
        private double ComboDeltaTimeCummulated { get; set; }
        public GameMap Map { get; set; }
        public WeatherController WeatherController { get; set;}
        public Player Player { get; set; }
        public Random Random { get; }
        public static GamestateManager Instance => instance ?? (instance = new GamestateManager());
        private HashSet<PointsAddedTextBox> PointsTextBoxes;
        public float WeatherModifier => WeatherController.GetSpeedModifier();
        public float WeatherTime => WeatherController.CurrentWeatherTime;

        private GamestateManager()
        {
            Points = 0;
            Combo = 1;
            Random = new Random();
            PointsTextBoxes = new HashSet<PointsAddedTextBox>();
            WeatherController = null;
        }

        public void Start()
        {
            WeatherController = new WeatherController();
        }

        public void AddPoints(long points, Vector2f? position = null, bool useCombo = true)
        {
            if (points < 0) PointsBeforeSubstraction = points;

            long pointsBeforeAddition = Points;
            if (useCombo)
            {
                ComboDeltaTimeCummulated = 0;
                Points += points * Combo;
                Combo = Math.Min(Combo + 1, MaxCombo);
            }
            else Points += points;

            if (pointsBeforeAddition / 5000 != Points / 5000 && Points > pointsBeforeAddition && Points > PointsBeforeSubstraction) Player.AddHealth(Convert.ToInt32((Points / 5000) - (pointsBeforeAddition / 5000)));

            PointsTextBoxes.Add(new PointsAddedTextBox(position ?? Player.Position + new Vector2f((Player.Size.X / 2) - 50, (Player.Size.Y / 4) - 10), points, useCombo ? Combo - 1 : 1));
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

        public void CompleteActivity(int points, Vector2f position)
        {
            AddPoints(points * Math.Max(++CompletedActivities, 1), position, false);
        }

        public void FailActivity(int points, Vector2f position)
        {
            AddPoints(-1 * points * Math.Max(--CompletedActivities, 1), position, false);
        }
    }
}