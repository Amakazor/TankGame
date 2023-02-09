using System;
using System.Text.Json.Serialization;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Brains;
using TankGame.Actors.Data;
using TankGame.Actors.Pawns.Enemies;
using TankGame.Actors.Pawns.Players;
using TankGame.Core.Gamestates;
using TankGame.Core.Map;
using TankGame.Core.Sounds;
using TankGame.Events;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Pawns;

public abstract class Pawn : TickableActor, IDestructible, ICoordinated {
    
    [JsonDerivedType(typeof(Player.Dto), typeDiscriminator: nameof(Player))]
    [JsonDerivedType(typeof(LightTank.Dto),  typeDiscriminator: nameof(LightTank))]
    [JsonDerivedType(typeof(MediumTank.Dto),  typeDiscriminator: nameof(MediumTank))]
    [JsonDerivedType(typeof(HeavyTank.Dto),  typeDiscriminator: nameof(HeavyTank))]
    public class Dto {
        public Direction Direction { get; set; }
        public Vector2f Position { get; set; }
        public Vector2f PreviousPosition { get; set; }
        public int Health { get; set; }
        public Brain.Dto? Brain { get; set; }

        [JsonIgnore] public Option<Brain.Dto> BrainOption {
            get => Optional(Brain);
            set => Brain = value.MatchUnsafe(b => b, () => null);
        }

        [JsonIgnore] public Vector2i Coords {
            get => (Vector2i)(Position / 64.0f);
            set => Position = (Vector2f)value * 64.0f;
        }

        public Dto WithCoords(Vector2i coords) {
            Coords = coords;
            return this;
        }
    }
    
    protected Pawn(Vector2i coords, Texture texture, int health, int sightDistance, float delay) : base((Vector2f)(coords * 64), new(64, 64)) {
        Health = health;
        PawnSprite = new(Position, Size, texture, new(255, 255, 255, 255));
        PawnSprite.SetRotation(Direction.Up);
        Brain = new(this, delay);
        BaseSightDistance = sightDistance;
        
        (this as IDestructible).RegisterDestructible();

        RenderLayer = RenderLayer.Pawn;
        RenderView = RenderView.Game;
    }

    protected Pawn(Dto dto, Texture texture, int sightDistance) : base(dto.Position, new(64, 64)) {
        PreviousPosition = dto.PreviousPosition;
        Health = dto.Health;
        Brain = new(this, dto.Brain);
        Direction = dto.Direction;
        PawnSprite = new(Position, Size, texture, new(255, 255, 255, 255));
        PawnSprite.SetRotation(Direction);
        BaseSightDistance = sightDistance;

        (this as IDestructible).RegisterDestructible();

        RenderLayer = RenderLayer.Pawn;
        RenderView = RenderView.Game;
    }

    public Direction Direction { get; set; }

    private SpriteComponent PawnSprite { get; }

    public Vector2f PreviousPosition { get; set; }

    public int BaseSightDistance { get; }

    public int SightDistance => (int) (BaseSightDistance * Gamestate.WeatherModifier);

    public int SquareSightDistance => SightDistance * SightDistance;

    public Brain Brain { get; set; }
    
    public Vector2i Coords {
        get => (Vector2i)(Position / 64.0f);
        set => Position = (Vector2f)value * 64.0f;
    }

    public Vector2i LastCoords => new((int)(PreviousPosition.X / Size.X), (int)(PreviousPosition.Y / Size.Y));

    public Option<Region> CurrentRegion => Gamestate.Level.GetRegionFromFieldCoords(Coords);

    public override System.Collections.Generic.HashSet<IRenderComponent> RenderComponents => new() { PawnSprite };

    public int Health { get; set; }

    public bool IsAlive => Health > 0;

    public bool IsDestructible => true;

    public Actor Actor => this;

    public DestructabilityType DestructabilityType => DestructabilityType.Destructible;
    public bool StopsProjectile => true;

    protected virtual void Register(Option<Region> region = default) {
        region.Match(reg => reg.FieldAt(Coords), () => Gamestate.Level.FieldAt(Coords)).IfSome(field => field.Pawn = this);
    }

    public virtual void Destroy() {
        MessageBus.PawnDeath.Invoke(this);
        Dispose();
    }

    public virtual void Hit() {
        SoundManager.PlayRandom(SoundType.Destruction, (Vector2f)Coords);
        if (IsDestructible && IsAlive) Health--;
        if (Health <= 0) Destroy();
    }

    public void SetRotation(float angle)
        => PawnSprite.SetRotation(angle);

    public void SetPosition(Vector2f position) {
        PawnSprite.SetPosition(position);
        Position = position;
    }

    public override void Dispose() {
        GC.SuppressFinalize(this);
        Brain.Dispose();
        ((IDestructible)this).UnregisterDestructible();
        base.Dispose();
    }
    
    public Direction GetDirectionFrom(Vector2i coords) {
        if (coords.X > Coords.X) return Direction.Left;
        if (coords.X < Coords.X) return Direction.Right;
        if (coords.Y > Coords.Y) return Direction.Up;
        return Direction.Down;
    }
    
    public virtual Dto ToDto() => new() {Direction = Direction, Position = Position, PreviousPosition = PreviousPosition, Health = Health, Brain = Brain.ToDto()};
}