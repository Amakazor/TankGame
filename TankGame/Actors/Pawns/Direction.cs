using SFML.System;
using TankGame.Core.Controls;
using TankGame.Extensions;

namespace TankGame.Actors.Pawns;

public enum Direction {
    Up,
    Down,
    Left,
    Right,
}

public static class DirectionExtensions {
    public static InputAction ToAction(this Direction direction)
        => direction switch {
            Direction.Up    => InputAction.MoveForward,
            Direction.Down  => InputAction.MoveBackwards,
            Direction.Left  => InputAction.RotateLeft,
            Direction.Right => InputAction.RotateRight,
            _               => InputAction.Nothing,
        };
    
    public static float ToAngle(this Direction direction)
        => direction switch {
            Direction.Up    => 180,
            Direction.Down  => 0,
            Direction.Left  => 90,
            Direction.Right => 270,
            _               => 0,
        };

    public static Direction RotateByAngle(this Direction direction, float angle) {
        float targetAngle = direction.ToAngle() + angle;
        while (targetAngle < 0) targetAngle += 360;
        
        int roundedAngle = ((int)targetAngle).RoundToNearest(90);
        int finalTargetAngle = roundedAngle % 360;

        return finalTargetAngle switch {
            0   => Direction.Down,
            90  => Direction.Left,
            180 => Direction.Up,
            270 => Direction.Right,
            _   => Direction.Down,
        };
    }
    
    public static Vector2f ToVector(this Direction direction)
        => direction switch {
            Direction.Down   => new(0, 1),
            Direction.Up    => new(0, -1),
            Direction.Left  => new(-1, 0),
            Direction.Right => new(1, 0),
            _               => new(-1, -1),
        };
    
    public static Vector2i ToIVector(this Direction direction)
        => direction switch {
            Direction.Down  => new(0, 1),
            Direction.Up    => new(0, -1),
            Direction.Left  => new(-1, 0),
            Direction.Right => new(1, 0),
            _               => new(-1, -1),
        };
}