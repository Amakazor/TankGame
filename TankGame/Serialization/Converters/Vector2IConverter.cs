using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SFML.System;

namespace TankGame.Serialization.Converters; 

public class Vector2IConverter : JsonConverter<Vector2i> {
    public override Vector2i Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var deserialized = (string)JsonSerializer.Deserialize(ref reader, typeof(string), options.IncludeFields ? options : new(options) { IncludeFields = true })!;
        var split = deserialized.Substring(1, deserialized.Length - 2).Split(';');
        if (split.Length != 2) throw new ArgumentOutOfRangeException();
                
        return parseInt(split[0]).Bind(x => parseInt(split[1]).Map(y => new Vector2i(x, y))).IfNone(() => throw new ArgumentOutOfRangeException());
    }

    public override void Write(Utf8JsonWriter writer, Vector2i value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, $"({value.X};{value.Y})", options);
}