using System;
using System.Text.Json.Serialization;

namespace TankGame.Actors.Data;

public class TraversabilityData : ICloneable {
    [JsonConstructor] public TraversabilityData(float speedModifier, bool isTraversible) {
        SpeedModifier = speedModifier;
        IsTraversible = isTraversible;
    }

    public float SpeedModifier { get; }
    public bool IsTraversible { get; }

    public object Clone()
        => MemberwiseClone();
}