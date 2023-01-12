using System.Text.Json.Serialization;

namespace TankGame.Core.Sounds;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MusicType {
    None,
    Rain,
    Snow,
}