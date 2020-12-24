using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Xml;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Data;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Fields
{
    internal class Field : Actor
    {
        public Vector2i Coords { get; }
        public FieldData FieldData { get; }
        private Texture Texture { get; }
        private SpriteComponent Surface { get; }
        public Pawn PawnOnField { get; set; }

        public Field(Vector2i coords, FieldData fieldData, Texture texture) : base(new Vector2f(coords.X * 64, coords.Y * 64), new Vector2f(64, 64))
        {
            Coords = coords;
            FieldData = fieldData;
            Texture = texture;
            Surface = new SpriteComponent(Position, Size, this, texture, new Color(255, 255, 255, 255));
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { Surface };
        }

        public bool IsTraversible()
        {
            return FieldData.IsTraversible && PawnOnField == null;
        }

        internal XmlElement SerializeToXML(XmlDocument xmlDocument)
        {
            if (FieldType.FieldTypes.ContainsValue(FieldData))
            {
                XmlElement fieldElement = xmlDocument.CreateElement("field");
                XmlElement typeElement = xmlDocument.CreateElement("type");
                XmlElement textureElement = xmlDocument.CreateElement("texture");

                foreach (KeyValuePair<string, FieldData> keyValuePair in FieldType.FieldTypes)
                {
                    typeElement.InnerText = keyValuePair.Key;
                }

                textureElement.InnerText = TextureManager.Instance.GetNameFromTexture(TextureType.Field, Texture);

                fieldElement.AppendChild(typeElement);
                fieldElement.AppendChild(textureElement);

                return fieldElement;
            }
            else throw new Exception();
        }
    }
}
