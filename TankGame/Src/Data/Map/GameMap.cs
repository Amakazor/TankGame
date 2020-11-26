using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TankGame.Src.Actors.Fields;

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

        public FieldData GetFieldDataFromRegion(Vector2i fieldCoords)
        {
            for (int i = 0; i < Regions.Count; i++)
            {
                Region region = Regions.ElementAt(i);

                if (region.HasField(fieldCoords))
                {
                    return region.GetFieldData(fieldCoords);
                }
            }

            throw new ArgumentException("There is no region that contains field of these coordinates", "fieldCoords");
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
    }
}
