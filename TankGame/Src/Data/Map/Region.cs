using SFML.System;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TankGame.Src.Actors.Fields;
using System.IO;
using System.Linq;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns;

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
        private List<Field> Fields { get; set; }
        private HashSet<Enemy> Enemies { get; set; }
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
                LoadEnemies();

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

            Fields = new List<Field>(from field in regionFile.Root.Element("fields").Descendants("field") select new Field(new Vector2i((Coords.X * FieldsInLine) + (i % FieldsInLine), (Coords.Y * FieldsInLine) + (i++ / FieldsInLine)), FieldType.FieldTypes[field.Element("type").Value], TextureManager.Instance.GetTexture(TextureType.Field, field.Element("texture").Value)));
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

            GetFieldAtMapCoords(Player.Coords).PawnOnField = Player;

            Console.WriteLine("Loaded player in region at " + Coords.X + " " + Coords.Y);
        }

        private void LoadEnemies()
        {
            Console.WriteLine("Loading enemies in region at " + Coords.X + " " + Coords.Y + "...");
            XDocument regionFile = XDocument.Load(RegionPathGenerator.GetRegionPath(Coords));

            if (regionFile.Root.Element("spawns") != null && regionFile.Root.Element("spawns").Descendants("enemy") != null)
            {
                Enemies = new HashSet<Enemy>(from enemy in regionFile.Root.Element("spawns").Descendants("enemy") select EnemyFactory.CreateEnemy(new Vector2f((Coords.X * FieldsInLine) + int.Parse(enemy.Element("x").Value), (Coords.Y * FieldsInLine) + int.Parse(enemy.Element("y").Value)), enemy.Element("type").Value, enemy.Element("aimc").Value));
            } else Enemies = new HashSet<Enemy>();

            Enemies.ToList().ForEach((Enemy enemy) => { GetFieldAtMapCoords(enemy.Coords).PawnOnField = enemy; });

            Console.WriteLine("Loaded " + Enemies.Count + " enemies in region at " + Coords.X + " " + Coords.Y);
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

        public bool HasField(Vector2i mapFieldCoords)
        {
            return Coords.X * FieldsInLine <= mapFieldCoords.X && Coords.Y * FieldsInLine <= mapFieldCoords.Y && Coords.X + 1 * FieldsInLine > mapFieldCoords.X && Coords.Y + 1 * FieldsInLine > mapFieldCoords.Y;
        }

        public Field GetFieldAtMapCoords(Vector2i mapFieldCoords)
        {
            if (HasField(mapFieldCoords))
            {
                return GetFieldAtIndex(ConvertRegionFieldCoordsToFieldIndex(ConvertMapCoordsToRegionFieldCoords(mapFieldCoords)));
            }
            else return null;
        }

        private Field GetFieldAtIndex(int index)
        {
            return Fields[index];
        }

        private Vector2i ConvertMapCoordsToRegionFieldCoords(Vector2i mapFieldCoords)
        {
            return new Vector2i(mapFieldCoords.X % FieldsInLine, mapFieldCoords.Y % FieldsInLine);
        }

        private int ConvertRegionFieldCoordsToFieldIndex(Vector2i regionFieldCoords)
        {
            return regionFieldCoords.X * FieldsInLine + regionFieldCoords.Y;
        }
    }
}
