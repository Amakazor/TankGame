using System.Text.Json.Serialization;

namespace TankGame.Core.Sounds;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum  SoundType {
    Destruction,
    Shot,
    Move,
}