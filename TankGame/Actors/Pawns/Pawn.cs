using System;
using System.Linq;
using System.Text.Json.Serialization;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Brains;
using TankGame.Actors.Data;
using TankGame.Actors.GameObjects;
using TankGame.Core.Gamestate;
using TankGame.Core.Map;
using TankGame.Core.Sounds;
using TankGame.Events;
using TankGame.Extensions;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Pawns;

public abstract class Pawn : TickableActor, IDestructible, ICoordinated {
    public class Dto {
        public Direction Direction { get; set; }
        public Vector2f RealPosition { get; set; }
    }
    
    protected Pawn(Vector2f position, Vector2f size, Texture texture, int? health, int sightDistance = 0) : base(position, size) {
        PawnSprite = new(Position, Size, texture, new(255, 255, 255, 255));
        PawnSprite.SetRotation(Direction.Up);
        BaseSightDistance = sightDistance != 0 ? sightDistance : 10;
        ((IDestructible)this).RegisterDestructible();

        RenderLayer = RenderLayer.Pawn;
        RenderView = RenderView.Game;
        
        Brain = new(this);
    }

    public Direction Direction { get; set; }
    private SpriteComponent PawnSprite { get; }
    public Vector2f PreviousPosition { get; set; }
    public Vector2f RealPosition { get; set; }
    public int BaseSightDistance { get; }
    public int SightDistance => (int) (BaseSightDistance * GamestateManager.WeatherModifier);
    public int SquareSightDistance => SightDistance * SightDistance;
    public Brain Brain { get; set; }

    public Vector2i Coords {
        get => new((int)(Position.X / Size.X), (int)(Position.Y / Size.Y));
        set => Position = new(value.X * Size.X, value.Y * Size.Y);
    }

    public Vector2i LastCoords => new((int)(PreviousPosition.X / Size.X), (int)(PreviousPosition.Y / Size.Y));

    public Option<Region> CurrentRegion => GamestateManager.Map.GetRegionFromFieldCoords(Coords);

    public override System.Collections.Generic.HashSet<IRenderComponent> RenderComponents => new() { PawnSprite };
    public int CurrentHealth { get; set; }

    public bool IsAlive => CurrentHealth > 0;
    public bool IsDestructible => true;
    public Actor Actor => this;
    public bool StopsProjectile => true;

    public virtual void Destroy() {
        MessageBus.PawnDeath.Invoke(this);
        Dispose();
    }

    public virtual void Hit() {
        SoundManager.PlayRandom(SoundType.Destruction, Position / 64);
        if (IsDestructible && IsAlive) CurrentHealth--;
        if (CurrentHealth <= 0) Destroy();
    }

    public void SetRotation(float angle)
        => PawnSprite.SetRotation(angle);

    public void SetPosition(Vector2f position) {
        PawnSprite.SetPosition(position);
        RealPosition = position;
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
}