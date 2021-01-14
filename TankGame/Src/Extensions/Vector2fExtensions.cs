using SFML.System;
using System;

namespace TankGame.Src.Extensions
{
    internal static class Vector2fExtensions
    {
        public static float ManhattanDistance(this Vector2f current, Vector2f other) => Math.Abs(current.X - other.X) + Math.Abs(current.Y - other.Y);
    }
}