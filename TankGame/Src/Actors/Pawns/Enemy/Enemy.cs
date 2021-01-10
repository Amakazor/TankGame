using SFML.Graphics;
using SFML.System;
using System.Xml;
using TankGame.Src.Actors.Pawns.MovementControllers;
using TankGame.Src.Data.Sounds;

namespace TankGame.Src.Actors.Pawns.Enemies
{
    abstract internal class Enemy : Pawn
    {
        public uint ScoreAdded { get; }
        public string Type { get; }
        public string MoveSoundName { get; }

        public Enemy(Vector2f position, Vector2f size, Texture texture, int health, uint scoreAdded, string type, string moveSoundName) : base(position, size, texture, health)
        {
            ScoreAdded = scoreAdded;
            Type = type;
            MoveSoundName = moveSoundName;
        }

        internal XmlNode SerializeToXML(XmlDocument xmlDocument)
        {
            XmlElement enemyElement = xmlDocument.CreateElement("enemy");
            XmlElement typeElement = xmlDocument.CreateElement("type");
            XmlElement xElement = xmlDocument.CreateElement("x");
            XmlElement yElement = xmlDocument.CreateElement("y");
            XmlElement healthElement = xmlDocument.CreateElement("health");

            typeElement.InnerText = Type;
            xElement.InnerText = (Coords.X % 20).ToString();
            yElement.InnerText = (Coords.Y % 20).ToString();
            healthElement.InnerText = Health.ToString();

            enemyElement.AppendChild(typeElement);
            enemyElement.AppendChild(xElement);
            enemyElement.AppendChild(yElement);
            enemyElement.AppendChild(healthElement);
            if (MovementController != null && MovementController is AIMovementController AIMC) enemyElement.AppendChild(AIMC.SerializeAIMovementControllerType(xmlDocument));
            if (MovementController != null && MovementController is PatrolAIMovementController PAIMC) enemyElement.AppendChild(PAIMC.SerializePath(xmlDocument));

            return enemyElement;
        }

        protected override void UpdatePosition(Vector2i lastCoords, Vector2i newCoords)
        {
            SoundManager.Instance.PlaySound("move", MoveSoundName, Position / 64);
            base.UpdatePosition(lastCoords, newCoords);
        }
    }
}