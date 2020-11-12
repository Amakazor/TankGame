using SFML.System;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TankGame.Src.Actors.Fields;
using System.IO;
using System.Linq;

namespace TankGame.Src.Data.Map
{
    internal static class RegionPathGenerator
    {
        private static readonly string DefaultRegionDirectory = "Resources/Region/";
        private static readonly string SavedRegionDirectory = "Resources/Save/Region/";

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
            else throw new Exception("File " + coords.X.ToString() + "_" + coords.Y.ToString() + ".xml could not be found");
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
        private HashSet<Field> Fields { get; set; }

        public Region(Vector2i coords)
        {
            Coords = coords;
            LoadFields();
        }

        private void LoadFields()
        {
            XDocument regionFile = XDocument.Load(RegionPathGenerator.GetRegionPath(Coords));
            int i = 0;
            Fields = new HashSet<Field>(from field in regionFile.Root.Element("fields").Descendants() select new Field(new Vector2i(i % 20, i++ / 20), FieldType.FieldTypes[field.Element("type").Value], TextureManager.Instance.GetTexture(TextureType.Field, field.Element("texture").Value)));
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
            Save();
        }
    }
}
