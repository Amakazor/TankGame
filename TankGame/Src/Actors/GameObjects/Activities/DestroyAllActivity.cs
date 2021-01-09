using SFML.System;
using System;
using System.Collections.Generic;
using System.Xml;
using TankGame.Src.Actors.Pawns.Enemies;

namespace TankGame.Src.Actors.GameObjects.Activities
{
    internal class DestroyAllActivity : Activity
    {
        public DestroyAllActivity(Vector2i coords, HashSet<Enemy> enemies, int? hp = null, string name = null, string type = null, int? pointsAdded = null, Tuple<TraversibilityData, DestructabilityData, string> gameObjectType = null) : base(coords, enemies, hp ?? 1, name??"Destroy all enemies", type??"destroy", gameObjectType??new Tuple<TraversibilityData, DestructabilityData, string>(new TraversibilityData(1, false), new DestructabilityData(1, false, false), null), pointsAdded??1000)
        {
            AllEnemiesCount = (uint)Enemies.Count;
            if (AllEnemiesCount == 0)
            {
                ActivityStatus = ActivityStatus.Completed;
                ChangeToCompleted();
            }
        }

        protected override string CalculateProgress()
        {
            if (Enemies.Count == 0 && ActivityStatus != ActivityStatus.Completed) ChangeStatus(ActivityStatus.Completed);

            if (ActivityStatus == ActivityStatus.Completed || ActivityStatus == ActivityStatus.Failed) return "";

            return "Enemy " + (AllEnemiesCount - Enemies.Count) + " of " + AllEnemiesCount;
        }

        internal override XmlElement SerializeToXML(XmlDocument xmlDocument)
        {
            XmlElement activityElement = xmlDocument.CreateElement("activity");
            XmlElement typeElement = xmlDocument.CreateElement("type");
            XmlElement xElement = xmlDocument.CreateElement("x");
            XmlElement yElement = xmlDocument.CreateElement("y");
            XmlElement healthElement = xmlDocument.CreateElement("health");

            typeElement.InnerText = Type;
            xElement.InnerText = (Coords.X % 20).ToString();
            yElement.InnerText = (Coords.Y % 20).ToString();
            healthElement.InnerText = Health.ToString();

            activityElement.AppendChild(typeElement);
            activityElement.AppendChild(xElement);
            activityElement.AppendChild(yElement);
            activityElement.AppendChild(healthElement);

            return activityElement;
        }
    }
}
