using SFML.System;

namespace TankGame.Utility;

public static class MathHelper {
    public static float Lerp(float start, float end, float amount) {
        return start + (end - start) * amount;
    }
    
    public static Vector2f Lerp(Vector2i start, Vector2i end, float amount) {
        return new Vector2f(Lerp(start.X, end.X, amount), Lerp(start.Y, end.Y, amount));
    }
    
    public static Vector2f Lerp(Vector2f start, Vector2f end, float amount) {
        return new Vector2f(Lerp(start.X, end.X, amount), Lerp(start.Y, end.Y, amount));
    }
}