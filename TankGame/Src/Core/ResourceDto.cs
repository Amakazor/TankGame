using System.Text.Json.Serialization;

namespace TankGame.Core.Sounds;

public class ResourceDto<T> {
    [JsonConstructor] public ResourceDto(T type, string name, string location) {
        Type = type;
        Name = name;
        Location = location;
    }

    public T Type { get; }
    public string Name { get; }
    public string Location { get; }
}