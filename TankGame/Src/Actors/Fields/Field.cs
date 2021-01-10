using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Xml;
using TankGame.Src.Actors.Data;
using TankGame.Src.Actors.GameObjects;
using TankGame.Src.Actors.GameObjects.Activities;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Data;
using TankGame.Src.Data.Gamestate;
using TankGame.Src.Data.Textures;
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
        public float TraversabilityMultiplier => TraversabilityData.SpeedModifier * ((GameObject != null && GameObject.IsTraversible) ? GameObject.TraversibilityData.SpeedModifier : 1);

        public Field(Vector2i coords, TraversibilityData traversabilityData, bool rotatable, Texture texture, string type, GameObject gameObject) : base(new Vector2f(coords.X * 64, coords.Y * 64), new Vector2f(64, 64))
        {
            Coords = coords;
            TraversabilityData = traversabilityData;
            Texture = texture;
            Type = type;
            Surface = new SpriteComponent(Position, Size, texture, new Color(255, 255, 255, 255));
            if (rotatable) Surface.SetDirection(GamestateManager.Instance.Random.Next(1, 5) * 90);
            GameObject = gameObject;
            if (gameObject != null) GameObject.Field = this;

            RenderLayer = RenderLayer.Field;
            RenderView = RenderView.Game;
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { Surface };
        }

        public bool IsTraversible(bool excludePlayer = false, bool orObjectDestructible = false)
        {
            return TraversabilityData.IsTraversible
                   && (PawnOnField == null || (excludePlayer && PawnOnField is Player))
                   && (GameObject != null ? (orObjectDestructible ? GameObject.IsDestructibleOrTraversible : GameObject.IsTraversible) : true);
        }

        public bool IsShootable(bool excludePlayer = false, bool orObjectDestructible = false)
        {
            return (PawnOnField == null || (excludePlayer && PawnOnField is Player))
                   && (GameObject != null ? (orObjectDestructible ? GameObject.IsDestructibleOrTraversible : GameObject.IsTraversible) : true);
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

            if (GameObject != null && !(GameObject is Activity)) fieldElement.AppendChild(GameObject.SerializeToXML(xmlDocument));

            return fieldElement;
        }

        public void OnGameObjectDestruction()
        {
            GameObject = null;
        }

        public override void Dispose()
        {
            if (GameObject != null) GameObject.Dispose();
            GameObject = null;

            base.Dispose();
        }
    }
}
