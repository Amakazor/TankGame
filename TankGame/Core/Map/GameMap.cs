using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Actors.Borders;
using TankGame.Actors.Fields;
using TankGame.Actors.Pawns.Enemies;
using TankGame.Actors.Pawns.Player;
using TankGame.Core.Gamestate;
using TankGame.Core.Textures;
using TankGame.Events;
using TankGame.Extensions;
using TankGame.Pathfinding;

namespace TankGame.Core.Map;

public class GameMap : IDisposable {
    private const int MapSize = 5;
    private const int FieldsInLine = 20;

    private static readonly JsonSerializerOptions SerializerOptions = new() {
        WriteIndented = true, 
        IncludeFields = true, 
        NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals, 
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, 
        PropertyNameCaseInsensitive = true,
    };

    public GameMap() {
        Borders = new() {
            new(new(-128, -128), new(64, 64), new(2, MapSize  * FieldsInLine + 4), TextureManager.Get(TextureType.Border, "hedgehog")),
            new(new(-128, -128), new(64, 64), new(MapSize * FieldsInLine + 4, 2), TextureManager.Get(TextureType.Border, "hedgehog")),
            new(new(MapSize       * FieldsInLine * 64, -128), new(64, 64), new(2, MapSize * FieldsInLine + 4), TextureManager.Get(TextureType.Border, "hedgehog")),
            new(new(-128, MapSize * FieldsInLine * 64), new(64, 64), new(MapSize * FieldsInLine + 4, 2), TextureManager.Get(TextureType.Border, "hedgehog")),
        };

        Vector2i playersRegionCoords = SearchForPlayerRegion();

        if (playersRegionCoords.IsValid())
            LoadRegionsAroundPlayer(playersRegionCoords);
        else
            throw new InvalidOperationException("No players region");

        MessageBus.PawnMoved += OnPawnMoved;
    }

    private HashSet<Region> Regions { get; } = new();
    private HashSet<Border> Borders { get; }

    public void Dispose() {
        GC.SuppressFinalize(this);

        MessageBus.PawnMoved -= OnPawnMoved;
        Regions.ToList()
               .ForEach(region => region.Dispose());

        Borders.ToList()
               .ForEach(border => border.Dispose());
    }

    public void Save()
        => Regions.ToList()
                  .ForEach(region => region.Save());

    public Region? GetRegionFromFieldCoords(Vector2i fieldCoords)
        => Regions.FirstOrDefault(region => region.HasField(fieldCoords));

    public Region? GetRegionFromMapCoords(Vector2i mapCoords)
        => Regions.FirstOrDefault(region => region.Coords.Equals(mapCoords));

    public Field? GetFieldFromRegion(Vector2i fieldCoords)
        => Regions.Where(region => region.HasField(fieldCoords))
                  .Select(region => region?.GetFieldAtMapCoords(fieldCoords))
                  .FirstOrDefault();

    public bool IsFieldTraversible(Vector2i fieldCoords, bool excludePlayer = false, bool orObjectDestructible = false) {
        Field? field = GetFieldFromRegion(fieldCoords);

        return field != null && field.IsTraversible(excludePlayer, orObjectDestructible);
    }

    private void LoadRegionsAroundPlayer(Vector2i coords) {
        if (!coords.IsValid()) return;

        UnloadRegionsOutsideOfPlayerVision(coords);

        Regions.UnionWith(NextCoords(coords)
                         .Where(nextCoord => !IsRegionLoaded(nextCoord) && nextCoord is { X: >= 0 and <= MapSize, Y: >= 0 and <= MapSize })
                         .Select(nextCoord => RegionPathGenerator.GetRegionPath(nextCoord))
                         .Select(File.ReadAllText!)
                         .Select(data => JsonSerializer.Deserialize<Region>(data, SerializerOptions))!
        );

        GamestateManager.Save();
    }

    private bool IsRegionLoaded(Vector2i coords)
        => GetRegionFromMapCoords(coords) is not null;

    private static IEnumerable<Vector2i> NextCoords(Vector2i coords) {
        List<Vector2i> nextCoords = new() {
            coords + new Vector2i(-1, -1),
            coords + new Vector2i(0, -1),
            coords + new Vector2i(1, -1),
            coords + new Vector2i(-1, 0),
            coords + new Vector2i(0, 0),
            coords + new Vector2i(1, 0),
            coords + new Vector2i(-1, 1),
            coords + new Vector2i(0, 1),
            coords + new Vector2i(1, 1),
        };
        return nextCoords;
    }

    private void UnloadRegionsOutsideOfPlayerVision(Vector2i coords)
        => Regions.ToList()
                  .ForEach(
                       region => {
                           if (region.Coords.X <= coords.X + 1 && region.Coords.X >= coords.X - 1 && region.Coords.Y <= coords.Y + 1 && region.Coords.Y >= coords.Y - 1) return;

                           region.Save();
                           region.Dispose();
                           Regions.Remove(region);
                       }
                   );

    private static Vector2i SearchForPlayerRegion() {
        for (var column = 0; column < MapSize; column++)
        for (var row = 0; row < MapSize; row++) {
            Vector2i regionCoords = new(column, row);
            string? regionPath = RegionPathGenerator.GetRegionPath(regionCoords);
            if (regionPath == null) continue;

            string fileContents = File.ReadAllText(regionPath);
            if (!fileContents.Contains("\"Player\": ") || fileContents.Contains("\"Player\": null")) continue;

            return regionCoords;
        }

        return new(-1, -1);
    }

    private IEnumerable<Vector2i> GenerateCoordsInRadius(Vector2i center, int radius) {
        List<Vector2i> coords = new();

        for (int x = center.X - radius; x <= center.X + radius; x++)
        for (int y = center.Y - radius; y <= center.Y + radius; y++)
            coords.Add(new(x, y));

        return coords;
    }

    public ISet<Node> GetNodesInRadius(Vector2i center, int radius) {
        return GenerateCoordsInRadius(center, radius)
              .Select(coords => (Coords: coords, Field: GetFieldFromRegion(coords)))
              .Select(data => data.Field is not null 
                          ? new Node(data.Coords, data.Field.IsTraversible(true), data.Field.TraversabilityMultiplier) 
                          : new Node(data.Coords, false))
              .ToHashSet();
    }

    private void OnPawnMoved(PawnMovedEventArgs eventArgs) {
        if (eventArgs.LastCoords.Equals(eventArgs.NewCoords)) return;

        Region? lastRegion = GetRegionFromFieldCoords(eventArgs.LastCoords);
        Region? newRegion = GetRegionFromFieldCoords(eventArgs.NewCoords);

        if (lastRegion.Equals(newRegion)) return;

        switch (eventArgs.Pawn) {
            case Player player:
                newRegion.AddPlayer(player);
                lastRegion.DeletePlayer();
                LoadRegionsAroundPlayer(newRegion.Coords);
                break;
            case Enemy enemy:
                newRegion.AddEnemy(enemy);
                lastRegion.DeleteEnemy(enemy);
                break;
        }
    }

    public static bool IsOutOfBounds(Vector2f position)
        => position.X < 0 || position.Y < 0 || position.X > MapSize * FieldsInLine * 64 || position.Y > MapSize * FieldsInLine * 64;
}