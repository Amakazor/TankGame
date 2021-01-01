using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Events;
using TankGame.Src.Extensions;
using TankGame.Src.Pathfinding;

namespace TankGame.Src.Data.Map
{
    internal class GameMap : IDisposable
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

            if (playersRegionCoords.IsValid())
            {
                Console.WriteLine("Players region found");
                LoadNineRegions(playersRegionCoords);
            }
            else
            {
                Console.WriteLine("Players region not founds");
                throw new Exception();
            }

            MessageBus.Instance.Register(MessageType.PawnMoved, OnPawnMoved);
        }

        public Region GetRegionFromFieldCoords(Vector2i fieldCoords)
        {
            for (int i = 0; i < Regions.Count; i++)
            {
                Region region = Regions.ElementAt(i);

                if (region.HasField(fieldCoords)) return region;
            }

            return null;
        }

        public Region GetRegionFromMapCoords(Vector2i mapCoords)
        {
            for (int i = 0; i < Regions.Count; i++)
            {
                Region region = Regions.ElementAt(i);

                if (region.Coords.Equals(mapCoords)) return region;
            }

            return null;
        }

        public Field GetFieldFromRegion(Vector2i fieldCoords)
        {
            for (int i = 0; i < Regions.Count; i++)
            {
                Region region = Regions.ElementAt(i);

                if (region.HasField(fieldCoords)) return region.GetFieldAtMapCoords(fieldCoords);
            }

            return null;
        }

        public bool IsFieldTraversible(Vector2i fieldCoords, bool excludePlayer = false, bool orObjectDestructible = false)
        {
            Field field = GetFieldFromRegion(fieldCoords);

            return field is null ? false : field.IsTraversible(excludePlayer, orObjectDestructible);
        }

        private void LoadNineRegions(Vector2i coords)
        {
            Console.WriteLine(coords.X.ToString());
            Console.WriteLine(coords.Y.ToString());

            if (coords.IsValid())
            {
                Regions.ToList().ForEach(region =>
                {
                    if (region.Coords.X > coords.X + 1 || region.Coords.X < coords.X - 1 || region.Coords.Y > coords.Y + 1 || region.Coords.Y < coords.Y - 1)
                    {
                        region.Dispose();
                        Regions.Remove(region);
                    }
                });

                for (int columnModifier = -1; columnModifier <= 1; columnModifier++) for (int rowModifier = -1; rowModifier <= 1; rowModifier++)
                {
                    Vector2i newCoords = new Vector2i(coords.X + columnModifier, coords.Y + rowModifier);

                    if (GetRegionFromMapCoords(newCoords) is null)
                    {
                        if (newCoords.X >= 0 && newCoords.X <= MapSize && newCoords.Y >= 0 && newCoords.Y <= MapSize && RegionPathGenerator.GetRegionPath(newCoords) != null)
                        {
                            Regions.Add(new Region(newCoords, FieldsInLine, true));
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

                    if (RegionPathGenerator.GetRegionPath(regionCoords) != null && new Region(regionCoords, FieldsInLine, false).ContainsPlayer())
                    {
                        return regionCoords;
                    }
                }
            }

            return new Vector2i(-1, -1);
        }

        private void DeleteSavedRegions()
        {
            var directory = Directory.GetParent(RegionPathGenerator.SavedRegionDirectory);

            //directory.EnumerateFiles().ToList().ForEach(file => file.Delete());
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

                    if (field != null) column.Add(new Node(new Vector2i(x - center.X + radius, y - center.Y + radius), field.IsTraversible(true), field.TraversabilityMultiplier));
                    else column.Add(new Node(new Vector2i(x - center.X + radius, y - center.Y + radius), false));
                }
                nodes.Add(column);
            }

            return nodes;
        }

        private void OnPawnMoved(object sender, EventArgs eventArgs)
        {
            if (eventArgs is PawnMovedEventArgs pawnMovedEventArgs && !pawnMovedEventArgs.LastCoords.Equals(pawnMovedEventArgs.NewCoords))
            {
                Region lastRegion = GetRegionFromFieldCoords(pawnMovedEventArgs.LastCoords);
                Region newRegion = GetRegionFromFieldCoords(pawnMovedEventArgs.NewCoords);
                if (!lastRegion.Equals(newRegion))
                {
                    if (sender is Player player)
                    {
                        newRegion.AddPlayer(player);
                        lastRegion.DeletePlayer();
                        LoadNineRegions(newRegion.Coords);
                    }
                    else if (sender is Enemy enemy)
                    {
                        newRegion.EnemyWanderedIn(enemy);
                        lastRegion.EnemyWanderedOut(enemy);
                    }
                }
            }
        }

        public void Dispose()
        {
            MessageBus.Instance.Unregister(MessageType.PawnMoved, OnPawnMoved);
        }
    }
}
