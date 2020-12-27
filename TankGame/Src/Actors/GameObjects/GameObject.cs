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
        public int Health { get => DestructabilityData.Health; set => DestructabilityData = new DestructabilityData(value, DestructabilityData.IsDestructible, DestructabilityData.DestroyOnEntry); }
        public bool IsTraversible => TraversibilityData.IsTraversible;
        public bool IsDestructible => DestructabilityData.IsDestructible;
        public bool IsDestructibleOrTraversible => IsDestructible || IsTraversible;
        public bool IsAlive => Health > 0;

        public GameObject(Vector2i coords, Tuple<TraversibilityData, DestructabilityData> gameObjectType, Texture texture, string type, int hp) : base(new Vector2f(coords.X * 64, coords.Y * 64), new Vector2f(64, 64))
        {
            TraversibilityData = gameObjectType.Item1;
            DestructabilityData = gameObjectType.Item2;
            Type = type;

            if (hp > 0) Health = hp;

            ObjectSprite = new SpriteComponent(Position, Size, this, texture, new Color(255, 255, 255, 255));
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { ObjectSprite };
        }

        public void OnDestroy(Actor other)
        {
            throw new NotImplementedException();
        }

        public void OnHit(Actor other)
        {
            throw new NotImplementedException();
        }

        internal XmlElement SerializeToXML(XmlDocument xmlDocument)
        {
            XmlElement objectElement = xmlDocument.CreateElement("object");
            XmlElement typeElement = xmlDocument.CreateElement("type");
            XmlElement hpElement = xmlDocument.CreateElement("hp");

            typeElement.InnerText = Type;
            hpElement.InnerText = DestructabilityData.Health.ToString();

            objectElement.AppendChild(typeElement);
            objectElement.AppendChild(hpElement);

            return objectElement;
        }
    }
}
