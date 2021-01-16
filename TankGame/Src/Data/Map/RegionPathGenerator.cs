using SFML.System;
using System.IO;

namespace TankGame.Src.Data.Map
{
    internal static class RegionPathGenerator
    {
        public static readonly string DefaultRegionDirectory = "Resources/Region/";
        public static readonly string SavedRegionDirectory = "Resources/Save/Region/";

        public static string GetDefaultRegionPath(Vector2i coords) => GenerateRegionPath(coords, DefaultRegionDirectory);
        public static string GetSavedRegionPath(Vector2i coords) => GenerateRegionPath(coords, SavedRegionDirectory);
        public static string GetFileName(Vector2i coords) => coords.X.ToString() + "_" + coords.Y.ToString() + ".xml";
        private static string GenerateRegionPath(Vector2i coords, string directory) => directory + GetFileName(coords);

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
    }
}
