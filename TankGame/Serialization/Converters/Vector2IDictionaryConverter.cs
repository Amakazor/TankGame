using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SFML.System;

namespace TankGame.Serialization.Converters; 

public class Vector2IDictionaryConverter<TValue> : JsonConverter<IDictionary<Vector2i, TValue>> {
    public override bool CanConvert(Type typeToConvert) {
        return typeToConvert == typeof(Dictionary<Vector2i, TValue>);
    }

    public override IDictionary<Vector2i, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var deserialized = (IEnumerable<KeyValuePair<Vector2i, TValue >>?)JsonSerializer.Deserialize(ref reader, typeof(IEnumerable<KeyValuePair<Vector2i, TValue>>), options);
        if (deserialized == null) return new Dictionary<Vector2i, TValue>();

        return new Dictionary<Vector2i, TValue>(deserialized);
    }

    public override void Write(Utf8JsonWriter writer, IDictionary<Vector2i, TValue> value, JsonSerializerOptions options) {
        JsonSerializer.Serialize(writer, value.Map(pair => pair), options);
    }
}