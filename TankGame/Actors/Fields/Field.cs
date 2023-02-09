using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Actors.Fields.Roads;
using TankGame.Actors.GameObjects;
using TankGame.Actors.Pawns;
using TankGame.Core.Gamestates;
using TankGame.Core.Textures;
using TankGame.Extensions;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Fields;

public abstract class Field : Actor, ITraversible, ICoordinated {
    private static readonly IEnumerable<Vector2i> MovementVectors = List<Vector2i>(new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(-1, -1), new(1, -1), new(-1, 1));
    public static readonly Texture EmptyTexture = TextureManager.Get(TextureType.Field, "empty");
    public static readonly Random Random = new();
    
    [JsonDerivedType(typeof(Grass.Dto),nameof(Grass))]
    [JsonDerivedType(typeof(Sand.Dto), nameof(Sand))]
    [JsonDerivedType(typeof(Road.Dto), nameof(Road))]
    [JsonDerivedType(typeof(Water.Dto),nameof(Water))]
    public class Dto {
        public GameObject.Dto? GameObject { get; set; }
        [JsonIgnore] public Option<GameObject.Dto> GameObjectOption {
            get => GameObject;
            set => GameObject = value.MatchUnsafe(val => val, () => null);
        }
        
        public int? TextureVariant { get; set; }
        [JsonIgnore] public Option<int> TextureVariantOption {
            get => Optional(TextureVariant);
            set => TextureVariant = value.MatchUnsafe<int?>(val => val, () => null);
        }
    }
    public Field(Vector2i coords, Texture texture, Option<GameObject> gameObject) : base((Vector2f)(coords * 64), new(64, 64)) {
        Surface = new(Position, Size, texture, new(255, 255, 255, 255));

        GameObject = gameObject;

        RenderLayer = RenderLayer.Field;
        RenderView = RenderView.Game;
    }

    public Field(Dto dto, Vector2i coords, Seq<Texture> textures) : this(dto, coords, GetTexture(dto.TextureVariantOption, textures)) { }

    public Field(Dto dto, Vector2i coords, Texture texture) : base((Vector2f)(coords * 64), new(64, 64)) {
        Surface = new(Position, Size, texture, new(255, 255, 255, 255));
        
        GameObject = dto.GameObjectOption.Map(gameObjectDto =>  GameObjectFactory.Create(gameObjectDto, coords));
        
        RenderLayer = RenderLayer.Field;
        RenderView = RenderView.Game;
    }

    public Vector2i Coords {
        get => new((int)(Position.X / 64), (int)(Position.Y / 64));
        set => throw new();
    }
    
    protected SpriteComponent Surface { get; set; }

    public Option<Pawn> Pawn { get; set; }

    public Option<GameObject> GameObject { get; set; }
    
    public abstract float BaseSpeedModifier { get; }
    public abstract bool BaseTraversible { get; }
    public float SpeedModifier => 
        BaseSpeedModifier * GameObject.Map(go => go.Traversible ? go.SpeedModifier : 1).IfNone(1);
    public bool Traversible => BaseTraversible && GameObject.Map(go => go.Traversible).IfNone(true);

    public override System.Collections.Generic.HashSet<IRenderComponent> RenderComponents => new() { Surface };
    
    public bool CanBeSpawnedOn()
        => Traversible;

    public bool CanBeShootThrough() => Pawn.IsNone && GameObject.Match(go => go.Traversible || go.DestructabilityType != DestructabilityType.Indestructible, true);

    public void DestroyObjectOnEntry(bool force = false) {
        if (GameObject.Map(go => force   || go.DestructabilityType == DestructabilityType.DestroyOnEntry)) GameObject.IfSome(go => go.Destroy());
    }

    public override void Dispose() {
        GC.SuppressFinalize(this);
        GameObject.IfSome(go => go.Dispose());
        base.Dispose();
    }

    public override string ToString() {
        return $"Field: {GetType().Name} [{Coords.X}, {Coords.Y}]";
    }

    public virtual Dto ToDto()
        => new() { GameObjectOption = GameObject.Map(go => go.ToDto()) };
    
    protected static Texture GetTexture(Option<int> variant, Seq<Texture> textures)
        => textures[variant.IfNone(Random.Next(textures.Count)) % textures.Count];

    public virtual void PostProcess() {}

    protected HashMap<DirectionFlag, Option<Field>> Neighbours()
        => MovementVectors
          .Map(vec => new KeyValuePair<DirectionFlag, Option<Field>>(vec.ToDirectionFlags(), Gamestate.Level.FieldAt(vec + Coords)))
          .ToHashMap();

    protected void CreateSurface(Texture texture) {
        Surface = new(Position, Size, texture, new(255, 255, 255, 255));
    }
}