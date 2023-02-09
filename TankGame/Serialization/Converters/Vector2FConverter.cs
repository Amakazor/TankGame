using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using SFML.System;

namespace TankGame.Serialization.Converters; 

public class Vector2FConverter : JsonConverter<Vector2f> {
    public override Vector2f Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var deserialized = (string)JsonSerializer.Deserialize(ref reader, typeof(string), options.IncludeFields ? options : new(options) { IncludeFields = true })!;
        var split = deserialized.Substring(1, deserialized.Length - 2).Split(';');
        if (split.Length != 2) throw new ArgumentOutOfRangeException();
                
        return parseFloat(split[0]).Bind(x => parseFloat(split[1]).Map(y => new Vector2f(x, y))).IfNone(() => throw new ArgumentOutOfRangeException());
    }

    public override void Write(Utf8JsonWriter writer, Vector2f value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, $"({value.X.ToString(CultureInfo.InvariantCulture)};{value.Y.ToString(CultureInfo.InvariantCulture)})", options);
}