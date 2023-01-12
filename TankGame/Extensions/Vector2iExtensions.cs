using System;
using System.Collections.Generic;
using SFML.System;

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

    public static Vector2i Modulo(this Vector2i current, int divisor)
        => new(current.X % divisor, current.Y % divisor);

    public static Vector2f ToVector2f(this Vector2i current)
        => new(current.X, current.Y);

    public static List<Vector2i> GetAllVectorsBeetween(this Vector2i current, Vector2i other) {
        if (IsInLine(current, other)) {
            var AllVectorsBeetween = new List<Vector2i>();

            if (ManhattanDistance(current, other) > 1) {
                if (IsInHorizontalLine(current, other)) {
                    if (current.X                 > other.X) (current, other) = (other, current);
                    for (int i = current.X + 1; i < other.X; i++) AllVectorsBeetween.Add(new(i, current.Y));
                } else {
                    if (current.Y                 > other.Y) (current, other) = (other, current);
                    for (int i = current.Y + 1; i < other.Y; i++) AllVectorsBeetween.Add(new(current.X, i));
                }
            }

            return AllVectorsBeetween;
        }

        return null;
    }
}