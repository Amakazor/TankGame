using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TankGame.Src.Actors.GameObjects;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Data;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Fields
{
    internal class Field : Actor
    {
        public Vector2i Coords { get; }
        public TraversibilityData TraversabilityData { get; }
        private Texture Texture { get; }
        private SpriteComponent Surface { get; }
        public Pawn PawnOnField { get; set; }
        public GameObject GameObject { get; set; }
        private string Type { get; }

        public Field(Vector2i coords, TraversibilityData traversabilityData, Texture texture, string type, GameObject gameObject) : base(new Vector2f(coords.X * 64, coords.Y * 64), new Vector2f(64, 64))
        {
            Coords = coords;
            TraversabilityData = traversabilityData;
            Texture = texture;
            Type = type;
            Surface = new SpriteComponent(Position, Size, this, texture, new Color(255, 255, 255, 255));
            GameObject = gameObject;
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { Surface };
        }

        public bool IsTraversible(bool excludePlayer = false, bool orObjectDestructible = false)
        {
            return TraversabilityData.IsTraversible
                   && (PawnOnField == null || (excludePlayer && PawnOnField is Player))
                   && (GameObject != null ? (orObjectDestructible ? GameObject.IsDestructibleOrTraversible : GameObject.TraversibilityData.IsTraversible) : true);
        }

        internal XmlElement SerializeToXML(XmlDocument xmlDocument)
        {
            XmlElement fieldElement = xmlDocument.CreateElement("field");
            XmlElement typeElement = xmlDocument.CreateElement("type");
            XmlElement textureElement = xmlDocument.CreateElement("texture");

            typeElement.InnerText = Type;
            textureElement.InnerText = TextureManager.Instance.GetNameFromTexture(TextureType.Field, Texture);

            fieldElement.AppendChild(typeElement);
            fieldElement.AppendChild(textureElement);

            if (GameObject != null) fieldElement.AppendChild(GameObject.SerializeToXML(xmlDocument));

            return fieldElement;
        }
    }
}
