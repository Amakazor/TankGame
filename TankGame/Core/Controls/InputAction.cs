using System.Text.Json.Serialization;

namespace TankGame.Core.Controls;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InputAction {
    Nothing,
    MoveForward,
    MoveBackwards,
    RotateLeft,
    RotateRight,
    Shoot,
    Pause,
}