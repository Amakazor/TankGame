using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Actors.GameObjects;
using TankGame.Src.Actors.GameObjects.Activities;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Events;
using TankGame.Src.Extensions;
using TankGame.Src.Pathfinding;

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
        public HashSet<Enemy> Enemies { get; private set; }
        private Player Player { get; set; }
        public Activity Activity { get; set; }
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
                MessageBus.Instance.Register(MessageType.PawnDeath, OnPawnDeath);
                LoadFields();
                LoadEnemies();
                if (ContainsPlayer() && GamestateManager.Instance.Player == null) LoadPlayer();
                LoadActivity();

                Loaded = true;
            }
        }

        public void Save()
        {
            if (Loaded)
            {
                XmlDocument savefile = new XmlDocument();
                XmlElement regionElement = savefile.CreateElement("region");

                regionElement.AppendChild(SerializeFields(savefile));
                regionElement.AppendChild(SerializeSpawns(savefile));
                regionElement.AppendChild(SerializeActivity(savefile));

                savefile.AppendChild(savefile.CreateXmlDeclaration("1.0", "utf-8", null));
                savefile.AppendChild(regionElement);
                savefile.Save(RegionPathGenerator.GetSavedRegionPath(Coords));
            }
        }

        public void Dispose()
        {
            if (Loaded)
            {
                Fields.ForEach(field => field.Dispose());
                Fields.Clear();

                Enemies.ToList().FindAll(enemy => enemy != null).ForEach(enemy => enemy.Dispose());
                Enemies.Clear();

                if (Player != null) Player.Dispose();

                Player = null;
                Activity = null;
                MessageBus.Instance.Unregister(MessageType.PawnDeath, OnPawnDeath);
            }
        }

        private void OnPawnDeath(object sender, EventArgs eventArgs)
        {
            if (eventArgs is PawnEventArgs pawnEventArgs && pawnEventArgs.Pawn is Enemy enemy && GetFieldAtMapCoords(enemy.Coords) != null)
            {
                DeleteEnemy(enemy);
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
            XDocument regionFile = XDocument.Load(RegionPathGenerator.GetRegionPath(Coords));
            int x = 0;
            int y = 0;

            Fields = new List<Field>();

            regionFile.Root.Element("fields").Descendants("field").ToList().ForEach((XElement fieldElement) => {
                Fields.Add(new Field(
                    new Vector2i((Coords.X * FieldsInLine) + x, (Coords.Y * FieldsInLine) + y),
                    FieldType.FieldTypes[fieldElement.Element("type").Value].Item1,
                    FieldType.FieldTypes[fieldElement.Element("type").Value].Item2,
                    TextureManager.Instance.GetTexture(TextureType.Field, fieldElement.Element("texture").Value),
                    fieldElement.Element("type").Value,
                    (fieldElement.Element("object") != null && fieldElement.Element("object").Value != null) 
                        ? new GameObject(
                            new Vector2i((Coords.X * FieldsInLine) + x, (Coords.Y * FieldsInLine) + y),
                            GameObjectType.GameObjectTypes[fieldElement.Element("object").Element("type").Value],
                            TextureManager.Instance.GetTexture(TextureType.GameObject, fieldElement.Element("object").Element("type").Value),
                            fieldElement.Element("object").Element("type").Value,
                            (fieldElement.Element("object").Element("hp") != null && fieldElement.Element("object").Element("hp").Value != null)
                                ? int.Parse(fieldElement.Element("object").Element("hp").Value)
                                : -1)
                        : null));

                if (y == 19)
                {
                    y = 0;
                    x++;
                }
                else
                {
                    y++;
                }
            });
        }

        private void LoadPlayer()
        {
            XDocument regionFile = XDocument.Load(RegionPathGenerator.GetRegionPath(Coords));
            XElement playerData = regionFile.Root.Element("spawns").Element("player");

            Player = new Player(new Vector2f(((Coords.X * FieldsInLine) + float.Parse(playerData.Element("x").Value)) * 64.0F, ((Coords.Y * FieldsInLine) + float.Parse(playerData.Element("y").Value)) * 64.0F), new Vector2f(64.0F, 64.0F));
            Player.Health = (playerData.Element("health") is null) ? Player.Health : int.Parse(playerData.Element("health").Value);

            GamestateManager.Instance.Player = Player;

            MessageBus.Instance.PostEvent(MessageType.PlayerHealthChanged, this, new PlayerHealthChangeEventArgs(Player.Health));
            if (Player.Health == 0) Player.OnDestroy();

            GetFieldAtMapCoords(Player.Coords).PawnOnField = Player;
        }

        private void LoadEnemies()
        {
            XDocument regionFile = XDocument.Load(RegionPathGenerator.GetRegionPath(Coords));

            if (regionFile.Root.Element("spawns") != null && regionFile.Root.Element("spawns").Descendants("enemy") != null)
            {
                Enemies = new HashSet<Enemy>(from enemy in regionFile.Root.Element("spawns").Descendants("enemy") select EnemyFactory.CreateEnemy(
                    new Vector2i((Coords.X * FieldsInLine) + int.Parse(enemy.Element("x").Value), (Coords.Y * FieldsInLine) + int.Parse(enemy.Element("y").Value)),
                    enemy.Element("type").Value,
                    enemy.Element("aimc").Value,
                    enemy.Element("path") != null && enemy.Element("path").Descendants("point") != null
                        ? new List<Vector2i>(from point in enemy.Element("path").Descendants("point") select new Vector2i((Coords.X * FieldsInLine) + int.Parse(point.Element("x").Value), (Coords.Y * FieldsInLine) + int.Parse(point.Element("y").Value)))
                        : null,
                    enemy.Element("health") != null 
                        ? int.Parse(enemy.Element("health").Value) 
                        : -1,
                    this));
            } else Enemies = new HashSet<Enemy>();
        }

        private void LoadActivity()
        {
            XDocument regionFile = XDocument.Load(RegionPathGenerator.GetRegionPath(Coords));
            XElement activityData = regionFile.Root.Element("activity");

            if (activityData != null && activityData.Element("type") != null)
            {
                int mapXCoords = (Coords.X * FieldsInLine);
                int mapYCoords = (Coords.Y * FieldsInLine);
                Vector2i ActivityCoords = new Vector2i(mapXCoords + int.Parse(activityData.Element("x").Value), mapYCoords + int.Parse(activityData.Element("y").Value));
                Activity = activityData.Element("type").Value switch
                {
                    "destroy" => new DestroyAllActivity(ActivityCoords, Enemies),
                    "protect" => new ProtectActivity(ActivityCoords, Enemies, activityData.Element("health") != null ? int.Parse(activityData.Element("health").Value) : -1),
                    "wave"    => new WaveActivity(ActivityCoords, Enemies,
                                                  new Queue<List<EnemySpawnData>>(from waves in activityData.Element("waves").Descendants("wave") select new List<EnemySpawnData>(from spawnData in waves.Descendants("enemy") select new EnemySpawnData(
                                                      new Vector2i(mapXCoords + int.Parse(spawnData.Element("x").Value),
                                                                   mapYCoords + int.Parse(spawnData.Element("y").Value)),
                                                      spawnData.Element("type").Value,
                                                      spawnData.Element("aimc").Value,
                                                      spawnData.Element("path") != null && spawnData.Element("path").Descendants("point") != null
                                                        ? new List<Vector2i>(from point in spawnData.Element("path").Descendants("point") select new Vector2i(mapXCoords + int.Parse(point.Element("x").Value), mapYCoords + int.Parse(point.Element("y").Value)))
                                                        : null))),
                                                  this,
                                                  activityData.Element("currentWave") != null ? uint.Parse(activityData.Element("currentWave").Value) : 0),
                    "waveprotect"    => new WaveProtectActivity(ActivityCoords, Enemies,
                                                  new Queue<List<EnemySpawnData>>(from waves in activityData.Element("waves").Descendants("wave") select new List<EnemySpawnData>(from spawnData in waves.Descendants("enemy") select new EnemySpawnData(
                                                      new Vector2i(mapXCoords + int.Parse(spawnData.Element("x").Value),
                                                                   mapYCoords + int.Parse(spawnData.Element("y").Value)),
                                                      spawnData.Element("type").Value,
                                                      spawnData.Element("aimc").Value,
                                                      spawnData.Element("path") != null && spawnData.Element("path").Descendants("point") != null
                                                        ? new List<Vector2i>(from point in spawnData.Element("path").Descendants("point") select new Vector2i(mapXCoords + int.Parse(point.Element("x").Value), mapYCoords + int.Parse(point.Element("y").Value)))
                                                        : null))),
                                                  this,
                                                  activityData.Element("currentWave") != null ? uint.Parse(activityData.Element("currentWave").Value) : 0, 
                                                  activityData.Element("health") != null ? int.Parse(activityData.Element("health").Value) : -1),
                    _ => throw new NotImplementedException()
                };
                Activity.Field = GetFieldAtMapCoords(Activity.Coords);
                Activity.Field.GameObject = Activity;
                if (Player != null && Activity.ActivityStatus == ActivityStatus.Stopped) Activity.ChangeStatus(ActivityStatus.Started);

            }
        }

        private XmlElement SerializeFields(XmlDocument xmlDocument)
        {
            XmlElement fieldsElement = xmlDocument.CreateElement("fields");

            foreach (Field field in Fields)
            {
                fieldsElement.AppendChild(field.SerializeToXML(xmlDocument));
            }

            return fieldsElement;
        }
        
        private XmlElement SerializeSpawns(XmlDocument xmlDocument)
        {
            XmlElement fieldsElement = xmlDocument.CreateElement("spawns");

            Enemies.ToList().FindAll(enemy => enemy != null).ForEach(enemy => fieldsElement.AppendChild(enemy.SerializeToXML(xmlDocument)));

            if (Player != null) fieldsElement.AppendChild(Player.SerializeToXML(xmlDocument));

            return fieldsElement;
        }

        private XmlNode SerializeActivity(XmlDocument savefile)
        {
            return Activity != null ? Activity.SerializeToXML(savefile) : savefile.CreateElement("activity");
        }

        public bool HasField(Vector2i mapFieldCoords)
        {
            return Coords.X * FieldsInLine <= mapFieldCoords.X && Coords.Y * FieldsInLine <= mapFieldCoords.Y && (Coords.X + 1) * FieldsInLine > mapFieldCoords.X && (Coords.Y + 1) * FieldsInLine > mapFieldCoords.Y;
        }

        public Field GetFieldAtMapCoords(Vector2i mapFieldCoords) => HasField(mapFieldCoords) ? FieldAtIndex(ConvertRegionFieldCoordsToFieldIndex(ConvertMapCoordsToRegionFieldCoords(mapFieldCoords))) : null;
        private Field FieldAtIndex(int index) => Fields[index];
        public Vector2i ConvertMapCoordsToRegionFieldCoords(Vector2i mapFieldCoords) => mapFieldCoords.Modulo(FieldsInLine);
        private int ConvertRegionFieldCoordsToFieldIndex(Vector2i regionFieldCoords) => regionFieldCoords.X * FieldsInLine + regionFieldCoords.Y;
        public bool HasDestructibleActivity => Activity != null && Activity.IsDestructible && Activity.ActivityStatus == ActivityStatus.Started;

        public List<List<Node>> GetNodesInRegion()
        {
            List<List<Node>> nodes = new List<List<Node>>();

            for (int x = 0; x < FieldsInLine; x++)
            {
                List<Node> column = new List<Node>();
                for (int y = 0; y < FieldsInLine; y++)
                {
                    Field field = Fields[ConvertRegionFieldCoordsToFieldIndex(new Vector2i(x, y))];

                    if (field != null) column.Add(new Node(new Vector2i(x, y), field.IsTraversible(true), field.TraversabilityMultiplier));
                    else column.Add(new Node(new Vector2i(x, y), false));
                }
                nodes.Add(column);
            }

            return nodes;
        }

        public void EnemyWanderedIn(Enemy enemy)
        {
            if (Activity != null) Activity.OnEnemyWanderIn();
            AddEnemy(enemy);
        }
        
        public void EnemyWanderedOut(Enemy enemy)
        {
            if (Activity != null) Activity.OnEnemyWanderOut();
            DeleteEnemy(enemy);
        }

        public void DeletePlayer()
        {
            Player = null;
            if (Activity != null && Activity.ActivityStatus == ActivityStatus.Started) Activity.ChangeStatus(ActivityStatus.Stopped);
        }

        public void AddPlayer(Player player)
        {
            Player = player;
            if (Activity != null && Activity.ActivityStatus == ActivityStatus.Stopped) Activity.ChangeStatus(ActivityStatus.Started);
        }

        public void DeleteEnemy(Enemy enemy)
        {
            Enemies.Remove(enemy);
            GameMap map = GamestateManager.Instance.Map;

            if (map.GetFieldFromRegion(enemy.Coords).PawnOnField == enemy) map.GetFieldFromRegion(enemy.Coords).PawnOnField = null;
            if (map.GetFieldFromRegion(enemy.LastCoords).PawnOnField == enemy) map.GetFieldFromRegion(enemy.LastCoords).PawnOnField = null;
        }

        public void AddEnemy(Enemy enemy) => Enemies.Add(enemy);
    }
}
