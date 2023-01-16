using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Actors.GameObjects;
using TankGame.Actors.GameObjects.Activities;
using TankGame.Actors.Pawns;
using TankGame.Actors.Pawns.Player;
using TankGame.Core.Textures;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Fields;

public class Field : Actor {
    [JsonConstructor] public Field(Vector2i coords, string textureName, GameObject? gObject, string type) : base(new(coords.X * 64, coords.Y * 64), new(64, 64)) {
        Coords = coords;
        Type = type;
        FieldType = FieldTypes.Types[type];
        Texture = TextureManager.Get(TextureType.Field, textureName);
        Surface = new(Position, Size, Texture, new(255, 255, 255, 255));

        GameObject = gObject;
        if (GameObject != null) GameObject.Field = this;

        RenderLayer = RenderLayer.Field;
        RenderView = RenderView.Game;

        RenderComponents = new() { Surface };
    }

    public Vector2i Coords { get; }

    [JsonIgnore] public FieldType FieldType { get; }

    [JsonIgnore] private Texture Texture { get; }
    public string TextureName => TextureManager.GetName(TextureType.Field, Texture);
    [JsonIgnore] private SpriteComponent Surface { get; }

    [JsonIgnore] public Pawn? PawnOnField { get; set; }

    [JsonIgnore] public GameObject? GameObject { get; set; }

    public GameObject? GObject => GameObject is not Activity ? GameObject : null;

    public string Type { get; set; }

    [JsonIgnore] public float TraversabilityMultiplier => FieldType.TraversabilityData.SpeedModifier * (GameObject is { IsTraversible: true } ? GameObject.GameObjectType.TraversabilityData.SpeedModifier : 1);

    [JsonIgnore] public override HashSet<IRenderComponent> RenderComponents { get; }

    public bool IsTraversible(bool excludePlayer = false, bool orObjectDestructible = false)
        => FieldType.TraversabilityData.IsTraversible && (PawnOnField == null || (excludePlayer && PawnOnField is Player)) && (GameObject == null || (orObjectDestructible ? GameObject.IsDestructibleOrTraversible : GameObject.IsTraversible));

    public bool CanBeSpawnedOn()
        => IsTraversible(false, true);

    public bool CanBeShootThrough(bool byEnemy) => (PawnOnField == null || (byEnemy && PawnOnField is Player)) && (GameObject == null || GameObject.IsTraversible || GameObject.IsDestructible);

    public void DestroyObjectIfExists() {
        if (GameObject is { GameObjectType.DestructabilityData.DestroyOnEntry: true }) GameObject.OnDestroy();
    }

    public void OnGameObjectDestruction()
        => GameObject = null;

    public override void Dispose() {
        GC.SuppressFinalize(this);

        GameObject?.Dispose();
        GameObject = null;

        base.Dispose();
    }

    public override string ToString() {
        return $"Field: {Type} [{Coords.X}, {Coords.Y}]";
    }
}