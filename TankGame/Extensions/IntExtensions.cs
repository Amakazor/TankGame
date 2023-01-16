using System;

namespace TankGame.Extensions; 

public static class IntExtensions {
    public static int RoundToNearest(this int value, int nearest)
        => (int) Math.Round(value / (double) nearest) * nearest;
}