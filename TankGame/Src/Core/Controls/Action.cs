using System.Text.Json.Serialization;

namespace TankGame.Core.Controls;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Action {
    Nothing,
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    Fire,
    Pause,
}