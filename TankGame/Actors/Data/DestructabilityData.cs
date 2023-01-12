using System;
using System.Text.Json.Serialization;

namespace TankGame.Actors.Data;

public class DestructabilityData : ICloneable {
    [JsonConstructor] public DestructabilityData(int health, bool isDestructible, bool destroyOnEntry, bool stopsProjectile) {
        Health = health;
        IsDestructible = isDestructible;
        DestroyOnEntry = destroyOnEntry;
        StopsProjectile = stopsProjectile;
    }

    public int Health { get; set; }
    public bool IsDestructible { get; }
    public bool DestroyOnEntry { get; }
    public bool StopsProjectile { get; }

    public object Clone()
        => MemberwiseClone();
}