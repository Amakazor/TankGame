using System;

namespace TankGame.Actors.Fields.Roads; 

[Flags]
public enum RoadDirection {
    None = 0,
    Up = 1,
    Right = 2,
    Down = 4,
    Left = 8,
}