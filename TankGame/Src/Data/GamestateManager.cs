using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Actors.Text;
using TankGame.Src.Data.Map;
using TankGame.Src.Events;

namespace TankGame.Src.Data
{
    internal class GamestateManager
    {
        private const double ComboTime = 7.5;
        private const uint MaxCombo = 10;
        private const string Savefile = "Resources/Save/currentsave.xml";
        private const uint PointsPerHealthUp = 5000;

        private static GamestateManager instance;

        public GamePhase GamePhase;

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
            MessageBus.Instance.Register(MessageType.PawnDeath, OnPawnDeath);
            GamePhase = GamePhase.NotStarted;

            Points = 0;
            Combo = 1;
            CompletedActivities = 0;

            Random = new Random();

            PointsTextBoxes = new HashSet<PointsAddedTextBox>();

            WeatherController = null;
        }

        private void OnPawnDeath(object sender, EventArgs eventArgs)
        {
            if (eventArgs is PawnEventArgs pawnEventArgs && pawnEventArgs.Pawn is Enemy enemy) AddPoints(enemy.ScoreAdded, enemy.RealPosition + new Vector2f((enemy.Size.X / 2) - 75, ((enemy.Size.Y / 10) - 10)));
        }

        public void Start(bool isNewGame)
        {
            GamePhase = GamePhase.Playing;
            WeatherController = new WeatherController();

            if (!File.Exists(Savefile) || isNewGame) DeleteSave();
            else Load();
            
            Map = new GameMap();

            Save();
        }

        public void Save()
        {
            if (!Directory.Exists("Resources/Save")) Directory.CreateDirectory("Resources/Save");
            if (!Directory.Exists("Resources/Save/Region")) Directory.CreateDirectory("Resources/Save/Region");

            XmlDocument savefile = new XmlDocument();

            XmlElement gamestateElement = savefile.CreateElement("gamestate");

            XmlElement pointsElement = savefile.CreateElement("points");
            XmlElement pointsBeforeSubstractionElement = savefile.CreateElement("pointsbs");
            XmlElement comboElement = savefile.CreateElement("combo");
            XmlElement completedActivitiesElement = savefile.CreateElement("activities");
            XmlElement comboDeltaTimeElement = savefile.CreateElement("combotime");
            XmlElement weatherElement = savefile.CreateElement("weathertype");
            XmlElement weatherTimeElement = savefile.CreateElement("weathertime");

            pointsElement.InnerText = Points.ToString();
            pointsBeforeSubstractionElement.InnerText = PointsBeforeSubstraction.ToString();
            comboElement.InnerText = Combo.ToString();
            completedActivitiesElement.InnerText = CompletedActivities.ToString();
            comboDeltaTimeElement.InnerText = ComboDeltaTimeCummulated.ToString();
            weatherElement.InnerText = WeatherController.WeatherType;
            weatherTimeElement.InnerText = WeatherController.CurrentWeatherTime.ToString();

            gamestateElement.AppendChild(pointsElement);
            gamestateElement.AppendChild(pointsBeforeSubstractionElement);
            gamestateElement.AppendChild(comboElement);
            gamestateElement.AppendChild(completedActivitiesElement);
            gamestateElement.AppendChild(comboDeltaTimeElement);
            gamestateElement.AppendChild(weatherElement);
            gamestateElement.AppendChild(weatherTimeElement);

            savefile.AppendChild(savefile.CreateXmlDeclaration("1.0", "utf-8", null));
            savefile.AppendChild(gamestateElement);
            savefile.Save(Savefile);

            Map.Save();
        }

        public void Load()
        {
            try
            {
                XDocument saveFile = XDocument.Load(Savefile);
                Points = int.Parse(saveFile.Root.Element("points").Value);
                PointsBeforeSubstraction = int.Parse(saveFile.Root.Element("pointsbs").Value);
                Combo = uint.Parse(saveFile.Root.Element("combo").Value);
                CompletedActivities = int.Parse(saveFile.Root.Element("activities").Value);
                ComboDeltaTimeCummulated = float.Parse(saveFile.Root.Element("combotime").Value);
                WeatherController.SetWeather(saveFile.Root.Element("weathertype").Value, float.Parse(saveFile.Root.Element("weathertime").Value));
            }
            catch (Exception)
            {
                DeleteSave();
            }
        }

        public void Clear()
        {
            Map.Dispose();
            PointsTextBoxes.ToList().ForEach(pointsTextBox => pointsTextBox.Dispose());
            WeatherController.Dispose();

            Map = null;
            Player = null;
            WeatherController = null;
            PointsTextBoxes = new HashSet<PointsAddedTextBox>();

            GamePhase = GamePhase.NotStarted;
            Points = 0;
            Combo = 1;
            CompletedActivities = 0;
            ComboDeltaTimeCummulated = 0;
        }

        public void DeleteSave()
        {
            DirectoryInfo directory = Directory.GetParent(RegionPathGenerator.SavedRegionDirectory);
            if (directory.Exists)
            {
                directory.GetFiles().ToList().ForEach(fileInfo => fileInfo.Delete());
                Directory.GetParent(directory.ToString()).GetFiles().ToList().ForEach(fileInfo => fileInfo.Delete());
            }
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

            if (pointsBeforeAddition / PointsPerHealthUp != Points / PointsPerHealthUp && Points > pointsBeforeAddition && Points > PointsBeforeSubstraction) Player.AddHealth(Convert.ToInt32((Points / PointsPerHealthUp) - (pointsBeforeAddition / PointsPerHealthUp)));

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

        public void CompleteActivity(int points, Vector2f position) => AddPoints((int)(points * ((++CompletedActivities / 6F) + (5F / 6F))), position, false);
        public void FailActivity(int points, Vector2f position) => AddPoints(-1 * points, position, false);
    }
}