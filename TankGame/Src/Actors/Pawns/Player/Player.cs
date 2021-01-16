using SFML.System;
using System;
using System.Xml;
using TankGame.Src.Actors.Pawns.MovementControllers;
using TankGame.Src.Data.Sounds;
using TankGame.Src.Data.Textures;
using TankGame.Src.Events;

namespace TankGame.Src.Actors.Pawns.Player
{
    internal class Player : Pawn
    {
        public Player(Vector2f position, Vector2f size, int health = 5) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "player1"), health)
        {
            MovementController = new PlayerMovementController(0.5F, this);
            MessageBus.Instance.PostEvent(MessageType.PlayerMoved, this, new EventArgs());
            MessageBus.Instance.PostEvent(MessageType.PlayerHealthChanged, this, new PlayerHealthChangeEventArgs(Health));
        }

        protected override void UpdatePosition(Vector2i lastCoords, Vector2i newCoords)
        {
            SoundManager.Instance.PlaySound("move", "light", Position / 64);
            base.UpdatePosition(lastCoords, newCoords);
            MessageBus.Instance.PostEvent(MessageType.PlayerMoved, this, new EventArgs());
        }

        public void AddHealth(int amount)
        {
            Health = Math.Min(10, Health + amount);
            MessageBus.Instance.PostEvent(MessageType.PlayerHealthChanged, this, new PlayerHealthChangeEventArgs(Health));
        }

        public override void OnHit()
        {
            base.OnHit();
            MessageBus.Instance.PostEvent(MessageType.PlayerHealthChanged, this, new PlayerHealthChangeEventArgs(Health));
        }

        internal XmlNode SerializeToXML(XmlDocument xmlDocument)
        {
            XmlElement playerElement = xmlDocument.CreateElement("player");
            XmlElement xElement = xmlDocument.CreateElement("x");
            XmlElement yElement = xmlDocument.CreateElement("y");
            XmlElement healthElement = xmlDocument.CreateElement("health");

            xElement.InnerText = (Coords.X % 20).ToString();
            yElement.InnerText = (Coords.Y % 20).ToString();
            healthElement.InnerText = Health.ToString();

            playerElement.AppendChild(xElement);
            playerElement.AppendChild(yElement);
            playerElement.AppendChild(healthElement);

            return playerElement;
        }
    }
}