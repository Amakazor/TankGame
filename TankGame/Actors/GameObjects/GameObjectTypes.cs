#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TankGame.Actors.Data;

namespace TankGame.Actors.GameObjects;

public class GameObjectType : ICloneable {
    [JsonConstructor] public GameObjectType(TraversabilityData traversabilityData, DestructabilityData destructabilityData, string texture, GameObjectType? gameObjectTypeAfterDestruction = null) {
        TraversabilityData = traversabilityData;
        DestructabilityData = destructabilityData;
        GameObjectTypeAfterDestruction = gameObjectTypeAfterDestruction;
        Texture = texture;
    }

    public TraversabilityData TraversabilityData { get; }
    public DestructabilityData DestructabilityData { get; }
    public GameObjectType? GameObjectTypeAfterDestruction { get; }
    public string Texture { get; set; }

    public object Clone()
        => new GameObjectType((TraversabilityData)TraversabilityData.Clone(), (DestructabilityData)DestructabilityData.Clone(), Texture, (GameObjectType?)GameObjectTypeAfterDestruction?.Clone());
}

public static class GameObjectTypes {
    public static GameObjectType Rubble => new(new(1.2F, true), new(1, false, false, false), "Rubble");

    public static GameObjectType Stump => new(new(1.2F, true), new(1, false, false, false), "Stump");

    public static GameObjectType Building => new(new(0.0F, false), new(1, false, false, true), "BigBuilding");

    public static GameObjectType House => new(new(0.0F, false), new(4, true, false, true), "House", Rubble);

    public static GameObjectType Fence1 => new(new(1.5F, true), new(2, true, true, true), "FenceColor", Rubble);

    public static GameObjectType Fence2 => new(new(1.5F, true), new(2, true, true, true), "FenceBW", Rubble);

    public static GameObjectType Tree => new(new(2.0F, true), new(1, true, true, true), "TreeDeciduous", Stump);

    public static GameObjectType Tree2 => new(new(2.0F, true), new(1, true, true, true), "TreeConifer", Stump);

    public static GameObjectType TowerCompleted => new(new(1.2F, true), new(1, false, false, false), "towercompleted");

    public static GameObjectType TowerDestroyed => new(new(1.2F, true), new(1, false, false, false), "towerdestroyed");

    public static GameObjectType Wave => new(new(1.0F, false), new(1, false, false, true), "tower", TowerDestroyed);

    public static GameObjectType WaveProtect => new(new(1.2F, false), new(0, true, false, true), "tower", TowerDestroyed);

    public static GameObjectType Protect => new(new(1.2F, false), new(0, true, false, true), "tower", TowerDestroyed);

    public static GameObjectType Destroy => new(new(1.2F, false), new(1, false, false, true), "tower", TowerDestroyed);

    private static Dictionary<string, GameObjectType> Types { get; } = new() {
        { "Rubble", Rubble },
        { "Stump", Stump },
        { "BigBuilding", Building },
        { "House", House },
        { "FenceColor", Fence1 },
        { "FenceBW", Fence2 },
        { "TreeDeciduous", Tree },
        { "TreeConifer", Tree2 },
        { "TowerCompleted", TowerCompleted },
        { "TowerDestroyed", TowerDestroyed },
        { "WaveActivity", Wave },
        { "WaveProtectActivity", WaveProtect },
        { "ProtectActivity", Protect },
        { "DestroyActivity", Destroy },
    };

    public static GameObjectType Get(string name)
        => (GameObjectType)Types[name]
           .Clone();
}