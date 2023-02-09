using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using LanguageExt;
using SFML.System;
using TankGame.Actors.Borders;
using TankGame.Actors.Fields;
using TankGame.Actors.Pawns.Enemies;
using TankGame.Actors.Pawns.Players;
using TankGame.Core.Textures;
using TankGame.Events;
using TankGame.Extensions;
using TankGame.Pathfinding;
using TankGame.Serialization.Converters;

namespace TankGame.Core.Map;

public class Level : IDisposable {
    private const int MapSize = 1;
    public static readonly int FieldsInLine = 9;

    private static readonly JsonSerializerOptions SerializerOptions = new() {
        WriteIndented = true, 
        NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals, 
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, 
        PropertyNameCaseInsensitive = true,
        Converters = { new Vector2IDictionaryConverter<Field.Dto>(), new Vector2IConverter(), new Vector2FConverter() },
    };

    public Level() {
        Borders = new() {
            new(new(-128, -128), new(64, 64), new(2, MapSize  * FieldsInLine + 4), TextureManager.Get(TextureType.Border, "hedgehog")),
            new(new(-128, -128), new(64, 64), new(MapSize * FieldsInLine + 4, 2), TextureManager.Get(TextureType.Border, "hedgehog")),
            new(new(MapSize       * FieldsInLine * 64, -128), new(64, 64), new(2, MapSize * FieldsInLine + 4), TextureManager.Get(TextureType.Border, "hedgehog")),
            new(new(-128, MapSize * FieldsInLine * 64), new(64, 64), new(MapSize * FieldsInLine + 4, 2), TextureManager.Get(TextureType.Border, "hedgehog")),
        };
    }

    public void Load() {
        var playersRegionCoords = SearchForPlayerRegion();
        LoadRegionsAroundPlayer(playersRegionCoords.IfNone(() => throw new InvalidOperationException("No players region")));

        MessageBus.PawnMoved += OnPawnMoved;
    }

    public System.Collections.Generic.Dictionary<Vector2i, Region> Regions { get; } = new();
    private System.Collections.Generic.HashSet<Border> Borders { get; }

    public void Dispose() {
        GC.SuppressFinalize(this);

        MessageBus.PawnMoved -= OnPawnMoved;
        Regions.ToList()
               .ForEach(region => region.Value.Dispose());

        Borders.ToList()
               .ForEach(border => border.Dispose());
    }

    // public void Save()
    //     => Regions.ToList()
    //               .ForEach(region => region.Save());

    public Option<Region> GetRegionFromFieldCoords(Vector2i fieldCoords)
        => Regions.Values.FirstOrDefault(region => region.HasField(fieldCoords));

    public Option<Region> GetRegionFromMapCoords(Vector2i mapCoords)
        => Regions.FirstOrDefault(region => region.Value.Coords.Equals(mapCoords)).Value;

    public Option<Field> FieldAt(Vector2i fieldCoords)
        => Regions
          .Values
          .Map(region => region.FieldAt(fieldCoords))
          .Somes()
          .HeadOrNone();
    
    private void LoadRegionsAroundPlayer(Vector2i coords) {
        if (!coords.IsValid()) return;

        UnloadRegionsOutsideOfPlayerVision(coords);

        var newRegions = NextCoords(coords)
                        .Filter(nextCoord => !IsRegionLoaded(nextCoord) && nextCoord is { X: >= 0 and < MapSize, Y: >= 0 and < MapSize })
                        .Map(RegionPathGenerator.GetRegionPath)
                        .Map(File.ReadAllText)
                        .Map(data => JsonSerializer.Deserialize<Region.Dto>(data, SerializerOptions))
                        .Map(Optional)
                        .Somes() 
                        .Map(dto => new Region(dto));

        foreach (Region region in newRegions) {
            Regions.Add(region.Coords, region);
            region.PostProcess();
        }
            

        Gamestates.Gamestate.Save();
    }

    private bool IsRegionLoaded(Vector2i coords)
        => GetRegionFromMapCoords(coords).IsSome;

    private static Seq<Vector2i> NextCoords(Vector2i coords)
        => Seq(coords + new Vector2i(-1, -1), coords + new Vector2i(0, -1), coords + new Vector2i(1, -1), coords + new Vector2i(-1, 0), coords + new Vector2i(0, 0), coords + new Vector2i(1, 0), coords + new Vector2i(-1, 1), coords + new Vector2i(0, 1), coords + new Vector2i(1, 1));

    private void UnloadRegionsOutsideOfPlayerVision(Vector2i coords) {
        foreach (Region region in Regions.Values.Filter(region => !region.IsNextTo(coords))) {
            Regions.Remove(region.Coords);
            region.Dispose();
        }
    }

    private static Option<Vector2i> SearchForPlayerRegion() {
        for (var column = 0; column < MapSize; column++)
        for (var row = 0; row < MapSize; row++) {
            Vector2i regionCoords = new(column, row);
            string regionPath = RegionPathGenerator.GetRegionPath(regionCoords);

            string fileContents = File.ReadAllText(regionPath);
            if (!fileContents.Contains("\"Player\": ") || fileContents.Contains("\"Player\": null")) continue;

            return regionCoords;
        }

        return None;
    }

    private static IEnumerable<Vector2i> GenerateCoordsInRadius(Vector2i center, int radius) {
        List<Vector2i> coords = new();

        for (int x = center.X - radius; x <= center.X + radius; x++)
        for (int y = center.Y - radius; y <= center.Y + radius; y++)
            coords.Add(new(x, y));

        return coords;
    }

    public ISet<Node> GetNodesInRadius(Vector2i center, int radius)
        => GenerateCoordsInRadius(center, radius)
          .Map(coords => (Coords: coords, Field: FieldAt(coords)))
          .Map(data => data.Field.Match<Node>(f => new(data.Coords, f.Traversible, f.SpeedModifier), () => new(data.Coords, false)))
          .ToHashSet();

    private void OnPawnMoved(PawnMovedEventArgs eventArgs) {
        eventArgs.Regions.IfSome(
            regs => {
                (Region lastRegion, Region newRegion) = regs;
                if (lastRegion == newRegion) return;

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
        );
    }

    public static bool IsOutOfBounds(Vector2f position)
        => position.X < 0 || position.Y < 0 || position.X > MapSize * FieldsInLine * 64 || position.Y > MapSize * FieldsInLine * 64;
}