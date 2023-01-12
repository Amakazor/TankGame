using System.IO;
using SFML.System;

namespace TankGame.Core.Map;

public static class RegionPathGenerator {
    public const string DefaultRegionDirectory = "Resources/Region/";
    public const string SavedRegionDirectory = "Resources/Save/Region/";

    public static string GetDefaultRegionPath(Vector2i coords)
        => GenerateRegionPath(coords, DefaultRegionDirectory);

    public static string GetSavedRegionPath(Vector2i coords)
        => GenerateRegionPath(coords, SavedRegionDirectory);

    public static string GetFileName(Vector2i coords)
        => coords.X + "_" + coords.Y + ".json";

    private static string GenerateRegionPath(Vector2i coords, string directory)
        => directory + GetFileName(coords);

    public static string GetRegionPath(Vector2i coords)
        => File.Exists(SavedRegionDirectory + GetFileName(coords)) ? GetSavedRegionPath(coords) : GetDefaultRegionPath(coords);
}