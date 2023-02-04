using System;
using System.Text.Json.Serialization;
using LanguageExt;
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
        GameObject.IfSome(go => go.Field = this);

        RenderLayer = RenderLayer.Field;
        RenderView = RenderView.Game;

        RenderComponents = new() { Surface };
    }

    public Vector2i Coords { get; }

    [JsonIgnore] public FieldType FieldType { get; }

    [JsonIgnore] private Texture Texture { get; }
    public string TextureName => TextureManager.GetName(TextureType.Field, Texture);
    [JsonIgnore] private SpriteComponent Surface { get; }

    [JsonIgnore] public Option<Pawn> PawnOnField { get; set; }

    [JsonIgnore] public Option<GameObject> GameObject { get; set; }

    public Option<GameObject> GObject => GameObject.Match(go => go is not Activity ? Some(go) : None, () => None);

    public string Type { get; set; }

    [JsonIgnore] public float TraversabilityMultiplier => 
        FieldType.TraversabilityData.SpeedModifier 
        * GameObject.Match(go => go.Traversible ? go.GameObjectType.TraversabilityData.SpeedModifier : 1, 1);

    [JsonIgnore] public override System.Collections.Generic.HashSet<IRenderComponent> RenderComponents { get; }

    public bool IsTraversible(bool excludePlayer = false, bool orObjectDestructible = false)
        => FieldType.TraversabilityData.IsTraversible && (PawnOnField.IsSome || (excludePlayer && PawnOnField.Match(pawn => pawn is Player, false))) && GameObject.Match(go => go.Traversible || (orObjectDestructible && go.IsDestructible), true);

    public bool CanBeSpawnedOn()
        => IsTraversible(false, true);

    public bool CanBeShootThrough(bool byEnemy) => PawnOnField.Match(pawn => byEnemy && pawn is Player, true) && GameObject.Match(go => go.Traversible || go.IsDestructible, true);

    public void DestroyObjectOnEntry(bool force = false) {
        if (GameObject.Match(go => force || go.DestructibleOnEntry, false)) GameObject.IfSome(go => go.Destroy());
    }

    public void OnGameObjectDestruction()
        => GameObject = null;

    public override void Dispose() {
        GC.SuppressFinalize(this);
        GameObject.IfSome(go => go.Dispose());
        base.Dispose();
    }

    public override string ToString() {
        return $"Field: {Type} [{Coords.X}, {Coords.Y}]";
    }
}