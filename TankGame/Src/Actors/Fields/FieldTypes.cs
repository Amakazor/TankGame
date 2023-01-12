using System.Collections.Generic;
using System.Text.Json.Serialization;
using TankGame.Actors.Data;

namespace TankGame.Actors.Fields;

public class FieldType {
    [JsonConstructor] public FieldType(TraversabilityData traversabilityData, bool rotatable) {
        TraversabilityData = traversabilityData;
        Rotatable = rotatable;
    }

    public TraversabilityData TraversabilityData { get; set; }
    public bool Rotatable { get; set; }
}

public static class FieldTypes {
    public static readonly FieldType Empty = new(new(1, true), false);
    public static readonly FieldType Grass = new(new(1.33F, true), true);
    public static readonly FieldType Road = new(new(1, true), false);
    public static readonly FieldType Sand = new(new(2F, true), true);
    public static readonly FieldType Water = new(new(0, false), true);

    public static readonly Dictionary<string, FieldType> Types = new() {
        { "empty", Empty },
        { "grass", Grass },
        { "road", Road },
        { "sand", Sand },
        { "water", Water },
    };
}