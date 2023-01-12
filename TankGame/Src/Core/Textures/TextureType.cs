using System.Text.Json.Serialization;

namespace TankGame.Core.Textures;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TextureType {
    Pawn,
    Field,
    GameObject,
    Projectile,
    Weather,
    Hp,
    Border,
}