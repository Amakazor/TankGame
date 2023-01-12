using System;
using SFML.System;

namespace TankGame.Extensions;

public static class Vector2fExtensions {
    public static float ManhattanDistance(this Vector2f current, Vector2f other)
        => Math.Abs(current.X - other.X) + Math.Abs(current.Y - other.Y);
}