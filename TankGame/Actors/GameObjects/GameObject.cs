using System;
using System.Text.Json.Serialization;
using LanguageExt;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Actors.Fields;
using TankGame.Actors.Pawns;
using TankGame.Core.Gamestate;
using TankGame.Core.Map;
using TankGame.Core.Sounds;
using TankGame.Core.Textures;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.GameObjects;

public class GameObject : Actor, IDestructible, IRenderable, ICoordinated {
    [JsonConstructor] public GameObject(Vector2i coords, string type, int? health = null) : base(new(coords.X * 64, coords.Y * 64), new(64, 64)) {
        Type = type;
        GameObjectType = GameObjectTypes.Get(type);
        if (health is not null) GameObjectType.DestructabilityData.Health = (int)health;

        (this as IDestructible).RegisterDestructible();
        GenerateSprite();
        SetRenderData();
    }

    public GameObjectType GameObjectType { get; protected set; }
    [JsonIgnore] private SpriteComponent ObjectSprite { get; set; }
    [JsonIgnore] public Region Region { get; set; }
    public string Type { get; }

    public int? Health => GameObjectType.DestructabilityData.Health;

    [JsonIgnore] public bool Traversible => GameObjectType.TraversabilityData.IsTraversible;
    [JsonIgnore] public bool DestructibleOnEntry => GameObjectType.DestructabilityData.DestroyOnEntry;
    [JsonIgnore] public Field Field { get; set; }
    [JsonPropertyOrder(-10)] public Vector2i Coords {
        get => new((int)(Position.X / 64), (int)(Position.Y / 64));
        set => throw new();
    }
    
    [JsonIgnore] public override System.Collections.Generic.HashSet<IRenderComponent> RenderComponents => new() { ObjectSprite };

    [JsonIgnore] public int CurrentHealth {
        get => GameObjectType.DestructabilityData.Health;
        set {
            if (Coords.X == 29 && Coords.Y == 44) Console.WriteLine("");
            GameObjectType.DestructabilityData.Health = value;
            if (CurrentHealth == 0) Destroy();
        }
    }

    [JsonIgnore] public bool IsDestructible => GameObjectType.DestructabilityData.IsDestructible;
    [JsonIgnore] public bool IsAlive => CurrentHealth > 0;
    [JsonIgnore] public Actor Actor => this;
    [JsonIgnore] public bool StopsProjectile => GameObjectType.DestructabilityData.StopsProjectile;

    public virtual void Destroy() {
        if (GameObjectType.GameObjectTypeAfterDestruction is null) {
            Field.OnGameObjectDestruction();
            Dispose();
        } else {
            GameObjectType = GameObjectType.GameObjectTypeAfterDestruction;
            GenerateSprite();
        }
    }

    public void Hit() {
        SoundManager.PlayRandom(SoundType.Destruction, Position / 64);
        if (IsDestructible && IsAlive) CurrentHealth--;
    }

    private void SetRenderData() {
        RenderLayer = RenderLayer.GameObject;
        RenderView = RenderView.Game;
    }

    protected void GenerateSprite()
        => ObjectSprite = new(Position, Size, TextureManager.Get(TextureType.GameObject, GameObjectType.Texture), new(255, 255, 255, 255));


    public override void Dispose() {
        GC.SuppressFinalize(this);
        (this as IDestructible).UnregisterDestructible();
        base.Dispose();
    }
    
    public Direction GetDirectionFrom(Vector2i coords) {
        if (coords.X > Coords.X) return Direction.Left;
        if (coords.X < Coords.X) return Direction.Right;
        if (coords.Y > Coords.Y) return Direction.Up;
        return Direction.Down;
    }
}