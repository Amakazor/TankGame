using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Actors.Borders;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Data.Gamestate;
using TankGame.Src.Data.Textures;
using TankGame.Src.Events;
using TankGame.Src.Extensions;
using TankGame.Src.Pathfinding;

namespace TankGame.Src.Data.Map
{
    internal class GameMap : IDisposable
    {
        private const int MapSize = 5;
        private const int FieldsInLine = 20;

        private HashSet<Region> Regions { get; set; }
        private HashSet<Border> Borders { get; set; }

        public GameMap()
        {
            Borders = new HashSet<Border>
            {
                new Border(new Vector2f(-128, -128), new Vector2f(64, 64), new Vector2i(2, MapSize*FieldsInLine + 4), TextureManager.Instance.GetTexture(TextureType.Border, "hedgehog")),
                new Border(new Vector2f(-128, -128), new Vector2f(64, 64), new Vector2i(MapSize*FieldsInLine + 4, 2), TextureManager.Instance.GetTexture(TextureType.Border, "hedgehog")),
                new Border(new Vector2f(MapSize*FieldsInLine*64, -128), new Vector2f(64, 64), new Vector2i(2, MapSize*FieldsInLine + 4), TextureManager.Instance.GetTexture(TextureType.Border, "hedgehog")),
                new Border(new Vector2f(-128, MapSize*FieldsInLine*64), new Vector2f(64, 64), new Vector2i(MapSize*FieldsInLine + 4, 2), TextureManager.Instance.GetTexture(TextureType.Border, "hedgehog")),
            };

            Regions = new HashSet<Region>();

            Vector2i playersRegionCoords = SearchForPlayerRegion();

            if (playersRegionCoords.IsValid()) LoadNineRegions(playersRegionCoords, false);
            else throw new InvalidOperationException("No players region");

            MessageBus.Instance.Register(MessageType.PawnMoved, OnPawnMoved);
        }

        public void Save() => Regions.ToList().ForEach(region => region.Save());

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

            return field != null && field.IsTraversible(excludePlayer, orObjectDestructible);
        }

        private void LoadNineRegions(Vector2i coords, bool save = true)
        {
            if (coords.IsValid())
            {
                Regions.ToList().ForEach(region =>
                {
                    if (region.Coords.X > coords.X + 1 || region.Coords.X < coords.X - 1 || region.Coords.Y > coords.Y + 1 || region.Coords.Y < coords.Y - 1)
                    {
                        region.Save();
                        region.Dispose();
                        Regions.Remove(region);
                    }
                });

                for (int columnModifier = -1; columnModifier <= 1; columnModifier++) for (int rowModifier = -1; rowModifier <= 1; rowModifier++)
                {
                    Vector2i newCoords = new Vector2i(coords.X + columnModifier, coords.Y + rowModifier);

                    if (GetRegionFromMapCoords(newCoords) is null && newCoords.X >= 0 && newCoords.X <= MapSize && newCoords.Y >= 0 && newCoords.Y <= MapSize && RegionPathGenerator.GetRegionPath(newCoords) != null)
                    {
                        Regions.Add(new Region(newCoords, FieldsInLine, true));
                    }
                }
            }

            if (save) GamestateManager.Instance.Save();
        }

        private Vector2i SearchForPlayerRegion()
        {
            for (int column = 0; column < MapSize; column++) for (int row = 0; row < MapSize; row++)
            {
                Vector2i regionCoords = new Vector2i(column, row);

                if (RegionPathGenerator.GetRegionPath(regionCoords) != null && new Region(regionCoords, FieldsInLine, false).ContainsPlayer())
                {
                    return regionCoords;
                }
            }

            return new Vector2i(-1, -1);
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

        public bool IsOutOfBounds(Vector2f position) => position.X < 0 || position.Y < 0 || position.X > MapSize * FieldsInLine * 64 || position.Y > MapSize * FieldsInLine * 64;

        public void Dispose()
        {
            MessageBus.Instance.Unregister(MessageType.PawnMoved, OnPawnMoved);
            Regions.ToList().ForEach(region => region.Dispose());
            Regions = null;

            Borders.ToList().ForEach(border => border.Dispose());
            Borders = null;
        }
    }
}