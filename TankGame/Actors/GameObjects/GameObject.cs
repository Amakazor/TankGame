using System;
using System.Text.Json.Serialization;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Actors.GameObjects.Buildings;
using TankGame.Actors.GameObjects.Buildings.Activities;
using TankGame.Actors.GameObjects.Buildings.Towers;
using TankGame.Actors.Pawns;
using TankGame.Core.Gamestates;
using TankGame.Core.Sounds;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.GameObjects;

public abstract class GameObject : Actor, IDestructible, IRenderable, ICoordinated, ITraversible {
    
    [JsonDerivedType(typeof(BigBuilding.Dto),         nameof(BigBuilding))]
    [JsonDerivedType(typeof(ConiferousTree.Dto),      nameof(ConiferousTree))]
    [JsonDerivedType(typeof(DeciduousTree.Dto),       nameof(DeciduousTree))]
    [JsonDerivedType(typeof(FenceBw.Dto),             nameof(FenceBw))]
    [JsonDerivedType(typeof(FenceColor.Dto),          nameof(FenceColor))]
    [JsonDerivedType(typeof(House.Dto),               nameof(House))]
    [JsonDerivedType(typeof(Rubble.Dto),              nameof(Rubble))]
    [JsonDerivedType(typeof(Stump.Dto),               nameof(Stump))]
    [JsonDerivedType(typeof(CompletedTower.Dto),      nameof(CompletedTower))]
    [JsonDerivedType(typeof(DestroyedTower.Dto),      nameof(DestroyedTower))]
    [JsonDerivedType(typeof(WaveActivity.Dto),        nameof(WaveActivity))]
    [JsonDerivedType(typeof(WaveProtectActivity.Dto), nameof(WaveProtectActivity))]
    [JsonDerivedType(typeof(DestroyAllActivity.Dto),  nameof(DestroyAllActivity))]
    [JsonDerivedType(typeof(ProtectActivity.Dto),     nameof(ProtectActivity))]
    public class Dto {
        [JsonIgnore] public Option<int> HealthOption { get; set; }

        public int? Health {
            get => HealthOption.MatchUnsafe<int?>(i => i, () => null);
            set => HealthOption = Optional(value);
        }
    }

    protected int _health;
    
    protected GameObject(Vector2i coords, Texture texture, int health = 1) : base((Vector2f)(coords * 64), new(64, 64)) {
        Register();
        _health = health;
        ObjectSprite = new(Position, Size, texture, new(255, 255, 255, 255));
    }

    protected GameObject(Dto dto, Texture texture, Vector2i coords) : this(coords, texture)
        => dto.HealthOption.IfSome(h => _health = h);

    private SpriteComponent ObjectSprite { get; }
     
     public Vector2i Coords {
        get => new((int)(Position.X / 64), (int)(Position.Y / 64));
        set => throw new();
     }
    
     public override System.Collections.Generic.HashSet<IRenderComponent> RenderComponents => new() { ObjectSprite };

     public virtual int Health {
         get => _health;
         set { _health = value; if (value <= 0) Destroy(); }
     }

     public abstract DestructabilityType DestructabilityType { get; }
     public abstract bool StopsProjectile { get; }
     public Actor Actor => this;

    public virtual void Destroy() {
        Gamestate.Level.FieldAt(Coords).IfSome(field => field.GameObject = AfterDestruction);
        Dispose();
    }

    public void Hit() {
        SoundManager.PlayRandom(SoundType.Destruction, Position / 64);
        if (DestructabilityType == DestructabilityType.Destructible && (this as IDestructible).IsAlive) Health--;
    }

    private void Register() {
        (this as IDestructible).RegisterDestructible();
        RenderLayer = RenderLayer.GameObject;
        RenderView = RenderView.Game;
    }


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

    public abstract float SpeedModifier { get; }
    public abstract bool Traversible { get; }
    
    protected abstract Option<GameObject> AfterDestruction { get; }

    public virtual Dto ToDto()
        => new() {HealthOption = Health};
}