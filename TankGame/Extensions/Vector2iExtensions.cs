using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SFML.System;
using TankGame.Utility;

namespace TankGame.Extensions;

public static class Vector2IExtensions {
    public static bool IsInvalid(this Vector2i current)
        => current.Equals(new(-1, -1));

    public static bool IsValid(this Vector2i current)
        => !IsInvalid(current);
    
    public static bool IsInLine(this Vector2i current, Vector2i other)
        => IsInVerticalLine(current, other) || IsInHorizontalLine(current, other);

    public static bool IsInVerticalLine(this Vector2i current, Vector2i other)
        => current.X == other.X;
    
    public static bool IsInHorizontalLine(this Vector2i current, Vector2i other)
        => current.Y == other.Y;

    public static int ManhattanDistance(this Vector2i current, Vector2i other)
        => Math.Abs(current.X - other.X) + Math.Abs(current.Y - other.Y);
    public static int SquareEuclideanDistance(this Vector2i current, Vector2i other)
        => (current.X - other.X) * (current.X - other.X) + (current.Y - other.Y) * (current.Y - other.Y);
    public static int EuclideanDistance(this Vector2i current, Vector2i other)
        => (int)Math.Sqrt(SquareEuclideanDistance(current, other));
    public static Vector2i Modulo(this Vector2i current, int divisor)
        => new(current.X % divisor, current.Y % divisor);

    public static Vector2f ToVector2f(this Vector2i current)
        => new(current.X, current.Y);

    public static IEnumerable<Vector2i> GetAllVectorsBetween(this Vector2i current, Vector2i other) {
        int distance = current.EuclideanDistance(other);
        return Enumerable.Range(0, distance).Select(number => (Vector2i)MathHelper.Lerp(current, other, (float)number / distance));
    }
}