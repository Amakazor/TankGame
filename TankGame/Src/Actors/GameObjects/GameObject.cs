using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Xml;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.GameObjects
{
    internal class GameObject : Actor, IDestructible
    {
        public TraversibilityData TraversibilityData { get; private set; }
        public DestructabilityData DestructabilityData { get; private set; }
        private SpriteComponent ObjectSprite { get; set; }
        private string Type { get; }

        public GameObject(Vector2i coords, Tuple<TraversibilityData, DestructabilityData> gameObjectType, Texture texture, string type, int hp) : base(new Vector2f(coords.X * 64, coords.Y * 64), new Vector2f(64, 64))
        {
            TraversibilityData = gameObjectType.Item1;
            DestructabilityData = gameObjectType.Item2;

            Type = type;

            ObjectSprite = new SpriteComponent(Position, Size, this, texture, new Color(255, 255, 255, 255));

            if (hp > 0)
            {
                SetHealth(hp);
            }
        }


        public int GetHealth()
        {
            return DestructabilityData.HP;
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { ObjectSprite };
        }

        public bool IsDestructible()
        {
            return DestructabilityData.IsDestructible;
        }

        public bool IsAlive()
        {
            return DestructabilityData.HP > 0;
        }

        public void OnDestroy(Actor other)
        {
            throw new NotImplementedException();
        }

        public void OnHit(Actor other)
        {
            throw new NotImplementedException();
        }

        public void SetHealth(int amount)
        {
            DestructabilityData = new DestructabilityData(amount, DestructabilityData.IsDestructible, DestructabilityData.DestroyOnEntry);
        }

        public bool IsDestructibleOrTraversible()
        {
            return IsDestructible() || TraversibilityData.IsTraversible;
        }

        internal XmlElement SerializeToXML(XmlDocument xmlDocument)
        {
            XmlElement objectElement = xmlDocument.CreateElement("object");
            XmlElement typeElement = xmlDocument.CreateElement("type");
            XmlElement hpElement = xmlDocument.CreateElement("hp");

            typeElement.InnerText = Type;
            hpElement.InnerText = DestructabilityData.HP.ToString();

            objectElement.AppendChild(typeElement);
            objectElement.AppendChild(hpElement);

            return objectElement;
        }
    }
}
