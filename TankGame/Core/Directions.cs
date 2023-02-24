using System;

namespace TankGame.Actors.Fields.Roads; 

[Flags]
public enum Directions {
    None = 0,
    Top = 1,
    Right = 2,
    Bottom = 4,
    Left = 8,
    TopRight = 16,
    BottomRight = 32,
    BottomLeft = 64,
    TopLeft = 128,
}