using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankGame.Src.Extensions
{
    internal static class Vector2iExtensions
    {
        public static int ManhattanDistance(this Vector2i vector2i, Vector2i other)
        {
            return Math.Abs(vector2i.X - other.X) + Math.Abs(vector2i.Y - other.Y);
        }

        public static bool IsInLine(this Vector2i vector2i, Vector2i other)
        {
            return IsInVerticalLine(vector2i, other) || IsInHorizontalLine(vector2i, other);
        }

        public static bool IsInVerticalLine(this Vector2i vector2i, Vector2i other)
        {
            return vector2i.X == other.X;
        }

        public static bool IsInHorizontalLine(this Vector2i vector2i, Vector2i other)
        {
            return vector2i.Y == other.Y;
        }

        public static List<Vector2i> GetAllVectorsBeetween(this Vector2i vector2i, Vector2i other)
        {
            if (IsInLine(vector2i, other))
            {
                List<Vector2i> AllVectorsBeetween = new List<Vector2i>();

                if (ManhattanDistance(vector2i, other) > 1)
                {
                    if (IsInHorizontalLine(vector2i, other))
                    {
                        if (vector2i.X < other.X) for (int i = vector2i.X + 1; i < other.X; i++) AllVectorsBeetween.Add(new Vector2i(i, vector2i.Y));
                        else                      for (int i = vector2i.X - 1; i > other.X; i--) AllVectorsBeetween.Add(new Vector2i(i, vector2i.Y));
                    }
                    else
                    {
                        if (vector2i.Y < other.Y) for (int i = vector2i.Y + 1; i < other.Y; i++) AllVectorsBeetween.Add(new Vector2i(vector2i.X, i));
                        else                      for (int i = vector2i.Y - 1; i > other.Y; i--) AllVectorsBeetween.Add(new Vector2i(vector2i.X, i));
                    }
                }
                return AllVectorsBeetween;
            }
            return null;
        }
    }
}
