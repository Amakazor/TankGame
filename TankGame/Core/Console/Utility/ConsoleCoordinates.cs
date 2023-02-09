using LanguageExt;
using SFML.System;

namespace TankGame.Core.Console.Utility; 

public static class ConsoleCoordinates {
    public static Option<Vector2i> Parse(string inputX, string inputY, Vector2i relativeTo) {
        bool isXRelative = inputX.EndsWith("r");
        bool isYRelative = inputY.EndsWith("r");
        
        if (isXRelative) inputX = inputX[..^1];
        if (isYRelative) inputY = inputY[..^1];
        
        return parseInt(inputX).SelectMany(x => parseInt(inputY), (x, y) => new Vector2i(x + (isXRelative ? relativeTo.X : 0), y + (isYRelative ? relativeTo.Y : 0)));
    }
}