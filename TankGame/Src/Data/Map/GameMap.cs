using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Pathfinding;

namespace TankGame.Src.Data.Map
{
    internal class GameMap
    {
        private const int MapSize = 5;
        private const int FieldsInLine = 20;

        private HashSet<Region> Regions { get; }

        public GameMap(bool newGame)
        {
            if (newGame)
            {
                DeleteSavedRegions();
            }

            Regions = new HashSet<Region>();

            Console.WriteLine("Searching players region...");

            Vector2i playersRegionCoords = SearchForPlayerRegion();

            if (playersRegionCoords.X > -1 && playersRegionCoords.Y > -1)
            {
                Console.WriteLine("Players region found");
                LoadNineRegions(playersRegionCoords);
            }
            else
            {
                Console.WriteLine("Players region not founds");
                throw new Exception();
            }
        }

        public Field GetFieldFromRegion(Vector2i fieldCoords)
        {
            for (int i = 0; i < Regions.Count; i++)
            {
                Region region = Regions.ElementAt(i);

                if (region.HasField(fieldCoords))
                {
                    return region.GetFieldAtMapCoords(fieldCoords);
                }
            }

            return null;
        }

        public bool IsFieldTraversible(Vector2i fieldCoords, bool excludePlayer = false, bool orObjectDestructible = false)
        {
            Field field = GetFieldFromRegion(fieldCoords);

            if (field is null) return false;
            return field.IsTraversible(excludePlayer, orObjectDestructible);
        }

        private void LoadNineRegions(Vector2i coords)
        {
            if (coords.X > -1 && coords.Y > -1)
            {
                for (int columnModifier = -1; columnModifier <= 1; columnModifier++)
                {
                    for (int rowModifier = -1; rowModifier <= 1; rowModifier++)
                    {
                        Vector2i newCoords = new Vector2i(coords.X + columnModifier, coords.Y + rowModifier);

                        Console.WriteLine("Trying to load region at " + newCoords.X + " " + newCoords.Y);

                        if (newCoords.X >= 0 && newCoords.X <= MapSize && newCoords.Y >= 0 && newCoords.Y <= MapSize && RegionPathGenerator.GetRegionPath(newCoords) != null)
                        {
                            Regions.Add(new Region(newCoords, FieldsInLine, true));
                        }
                        else
                        {
                            Console.WriteLine("File for region at " + newCoords.X + " " + newCoords.Y + " could not be located");
                        }
                    }
                }
            }
        }

        private Vector2i SearchForPlayerRegion()
        {
            for (int column = 0; column < MapSize; column++)
            {
                for (int row = 0; row < MapSize; row++)
                {
                    Vector2i regionCoords = new Vector2i(column, row);

                    if (RegionPathGenerator.GetRegionPath(regionCoords) != null)
                    {
                        if (new Region(regionCoords, FieldsInLine, false).ContainsPlayer())
                        {
                            return regionCoords;
                        }
                    }
                }
            }

            return new Vector2i(-1, -1);
        }

        private void DeleteSavedRegions()
        {
            var directory = Directory.GetParent(RegionPathGenerator.SavedRegionDirectory);

            directory.EnumerateFiles().ToList().ForEach(file => file.Delete());
        }

        public List<List<Node>> GetNodesInRadius(Vector2i center, int radius)
        {
            List<List<Node>> nodes = new List<List<Node>>();

            for (int x = center.X - radius; x <= center.X + radius; x++)
            {
                List<Node> column = new List<Node>();
                for (int y = center.Y - radius; y <= center.Y + radius; y++)
                {
                    Field field = GetFieldFromRegion(new Vector2i(x, y));

                    if (field != null) column.Add(new Node(new Vector2i(x - center.X + radius, y - center.Y + radius), field.IsTraversible(true)));
                    else column.Add(new Node(new Vector2i(x - center.X + radius, y - center.Y + radius), false));
                }
                nodes.Add(column);
            }

            return nodes;
        }
    }
}
