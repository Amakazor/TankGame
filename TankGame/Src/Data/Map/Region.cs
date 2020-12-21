using SFML.System;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TankGame.Src.Actors.Fields;
using System.IO;
using System.Linq;
using TankGame.Src.Actors.Pawn.Player;

namespace TankGame.Src.Data.Map
{
    internal static class RegionPathGenerator
    {
        public static readonly string DefaultRegionDirectory = "Resources/Region/";
        public static readonly string SavedRegionDirectory = "Resources/Save/Region/";
        public static string GetRegionPath(Vector2i coords)
        {
            string regionFileName = GetFileName(coords);

            if (File.Exists(SavedRegionDirectory + regionFileName))
            {
                return GetSavedRegionPath(coords);
            }
            else if (File.Exists(DefaultRegionDirectory + regionFileName))
            {
                return GetDefaultRegionPath(coords);
            }
            else return null;
        }

        public static string GetDefaultRegionPath(Vector2i coords)
        {
            return GenerateRegionPath(coords, DefaultRegionDirectory);
        }
        
        public static string GetSavedRegionPath(Vector2i coords)
        {
            return GenerateRegionPath(coords, SavedRegionDirectory);
        }

        public static string GetFileName(Vector2i coords)
        {
            return coords.X.ToString() + "_" + coords.Y.ToString() + ".xml";
        }

        private static string GenerateRegionPath(Vector2i coords, string directory)
        {
            return directory + GetFileName(coords);
        }
    }

    internal class Region : IDisposable
    {
        public Vector2i Coords { get; }
        public int FieldsInLine { get; }
        private HashSet<Field> Fields { get; set; }
        private Player Player { get; set; }
        public bool Loaded { get; private set; }

        public Region(Vector2i coords, int fieldsInLine, bool load)
        {
            Coords = coords;
            FieldsInLine = fieldsInLine;

            if (load)
            {
                Load();
            }

            Console.WriteLine("Constructed region at: " + coords.X + " " + coords.Y);
        }

        public void Load()
        {
            if (!Loaded)
            {
                LoadFields();
                if (ContainsPlayer())
                {
                    LoadPlayer();
                }
            }
        }

        public bool ContainsPlayer()
        {
            XDocument regionFile = XDocument.Load(RegionPathGenerator.GetRegionPath(Coords));

            return regionFile != null
                && regionFile.Root.Element("spawns") != null
                && regionFile.Root.Element("spawns").Element("player") != null;
        }

        private void LoadFields()
        {
            Console.WriteLine("Loading fields in region at " + Coords.X + " " + Coords.Y + "...");
            XDocument regionFile = XDocument.Load(RegionPathGenerator.GetRegionPath(Coords));
            int i = 0;

            Fields = new HashSet<Field>(from field in regionFile.Root.Element("fields").Descendants("field") select new Field(new Vector2i((Coords.X * FieldsInLine) + (i % FieldsInLine), (Coords.Y * FieldsInLine) + (i++ / FieldsInLine)), FieldType.FieldTypes[field.Element("type").Value], TextureManager.Instance.GetTexture(TextureType.Field, field.Element("texture").Value)));
            Console.WriteLine("Loaded "+ i + " fields in region at " + Coords.X + " " + Coords.Y);
        }

        private void LoadPlayer()
        {
            Console.WriteLine("Loading player in region at " + Coords.X + " " + Coords.Y + "...");
            XDocument regionFile = XDocument.Load(RegionPathGenerator.GetRegionPath(Coords));
            XElement playerData = regionFile.Root.Element("spawns").Element("player");

            Player = new Player(new Vector2f(((Coords.X * FieldsInLine) + float.Parse(playerData.Element("x").Value)) * 64.0F, ((Coords.Y * FieldsInLine) + float.Parse(playerData.Element("y").Value)) * 64.0F), new Vector2f(64.0F, 64.0F));
            Player.SetHealth((playerData.Element("hp") is null) ? Player.DefaultPlayerHP : int.Parse(playerData.Element("hp").Value));

            GamestateManager.Instance.Player = Player;

            Console.WriteLine("Loaded player in region at " + Coords.X + " " + Coords.Y);
        }

        private XmlElement SerializeFields(XmlDocument xmlDocument)
        {
            XmlElement fieldsElement = xmlDocument.CreateElement("Fields");

            foreach (Field field in Fields)
            {
                fieldsElement.AppendChild(field.SerializeToXML(xmlDocument));
            }

            return fieldsElement;
        }

        private void Save()
        {
            XmlDocument savefile = new XmlDocument();
            savefile.AppendChild(savefile.CreateXmlDeclaration("1.0", "utf-8", null));
            savefile.AppendChild(SerializeFields(savefile));
            savefile.Save(RegionPathGenerator.GetSavedRegionPath(Coords));
        }

        public void Dispose()
        {
            if (Loaded)
            {
                Save();
            }
        }

        public FieldData GetFieldData(Vector2i fieldCoords)
        {
            for (int i = 0; i < FieldsInLine * FieldsInLine; i++)
            {
                Field field = Fields.ElementAt(i);

                if (field.Coords == fieldCoords)
                {
                    return field.FieldData;
                }
            }
            throw new ArgumentException("No field at those coordinates", "fieldCoords");
        }

        public bool HasField(Vector2i fieldCoords)
        {
            return Coords.X * FieldsInLine <= fieldCoords.X && Coords.Y * FieldsInLine <= fieldCoords.Y && Coords.X + 1 * FieldsInLine > fieldCoords.X && Coords.Y + 1 * FieldsInLine > fieldCoords.Y;
        }
    }
}
