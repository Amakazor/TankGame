using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Actors.Pawns.MovementControllers;
using TankGame.Core.Gamestate;
using TankGame.Core.Map;
using TankGame.Core.Sounds;
using TankGame.Events;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Pawns;

public abstract class Pawn : TickableActor, IDestructible {
    protected Pawn(Vector2f position, Vector2f size, Texture texture) : base(position, size) {
        PawnSprite = new(Position, Size, texture, new(255, 255, 255, 255));
        PawnSprite.SetDirection(Direction.Up);
        ((IDestructible)this).RegisterDestructible();

        RenderLayer = RenderLayer.Pawn;
        RenderView = RenderView.Game;
    }

    protected Pawn(Vector2f position, Vector2f size, Texture texture, int? health) : base(position, size) {
        PawnSprite = new(Position, Size, texture, new(255, 255, 255, 255));
        PawnSprite.SetDirection(Direction.Up);
        ((IDestructible)this).RegisterDestructible();

        RenderLayer = RenderLayer.Pawn;
        RenderView = RenderView.Game;
    }

    [JsonIgnore] public Direction PreviousDirection { get; set; }
    public Direction Direction { get; set; }
    public MovementController? MovementController { get; private set; }
    [JsonIgnore] private SpriteComponent PawnSprite { get; }
    [JsonIgnore] public Vector2f PreviousPosition { get; set; }
    [JsonIgnore] public Vector2f RealPosition => CalculatePosition();

    public Vector2i Coords {
        get => new((int)(Position.X / Size.X), (int)(Position.Y / Size.Y));
        set => Position = new(value.X * Size.X, value.Y * Size.Y);
    }

    [JsonIgnore] public Vector2i LastCoords => new((int)(PreviousPosition.X / Size.X), (int)(PreviousPosition.Y / Size.Y));

    [JsonIgnore] public Region? CurrentRegion => GamestateManager.Map.GetRegionFromFieldCoords(Coords);

    public override HashSet<IRenderComponent> RenderComponents => new() { PawnSprite };
    public int CurrentHealth { get; set; }

    [JsonIgnore] public bool IsAlive => CurrentHealth > 0;
    [JsonIgnore] public bool IsDestructible => true;
    [JsonIgnore] public Actor Actor => this;
    [JsonIgnore] public bool StopsProjectile => true;

    public virtual void OnDestroy() {
        MessageBus.PawnDeath.Invoke(this);
        Dispose();
    }

    public virtual void OnHit() {
        SoundManager.PlayRandomSound("destruction", Position / 64);
        if (IsDestructible && IsAlive) CurrentHealth--;
        if (CurrentHealth <= 0) OnDestroy();
    }

    public void AttachMovementController(MovementController controller) {
        if (MovementController is not null) return;
        MovementController = controller;
    }

    public override void Tick(float deltaTime) {
        PawnSprite.SetDirection(CalculateRotationAngle());
        PawnSprite.SetPosition(RealPosition);

        if (MovementController is not null) {
            if (MovementController.CanDoAction()) {
                Vector2i lastCoords = Coords;
                Direction = MovementController.DoAction(Direction);
                Vector2i newCoords = Coords;
                UpdatePosition(lastCoords, newCoords);
            } else if (MovementController is PlayerMovementController) { MovementController.ClearAction(); }

            MovementController.Tick(deltaTime);
        }
    }

    protected virtual void UpdatePosition(Vector2i lastCoords, Vector2i newCoords) {
        if (lastCoords == newCoords) return;

        PawnSprite.SetPosition(RealPosition);
        PawnSprite.SetDirection(CalculateRotationAngle());
        MessageBus.PawnMoved.Invoke(new(lastCoords, newCoords, this));
    }

    protected double CalculateRotationAngle() {
        if (MovementController is null || !MovementController.IsRotating) return GetRotationAngleFromDirection(Direction);

        double startRotationAngle = GetRotationAngleFromDirection(PreviousDirection);
        double endRotationAngle = GetRotationAngleFromDirection(Direction);

        if (Math.Abs(startRotationAngle + 360 - endRotationAngle)   < Math.Abs(startRotationAngle - endRotationAngle)) startRotationAngle += 360;
        if (Math.Abs(endRotationAngle   + 360 - startRotationAngle) < Math.Abs(endRotationAngle   - startRotationAngle)) endRotationAngle += 360;

        return startRotationAngle + (endRotationAngle - startRotationAngle) * MovementController.RotationProgress;
    }

    protected Vector2f CalculatePosition()
        => MovementController is not null && MovementController.IsMoving ? PreviousPosition + (Position - PreviousPosition) * (float)MovementController.MovementProgress : Position;

    protected static float GetRotationAngleFromDirection(Direction direction)
        => direction switch {
            Direction.Up    => 180,
            Direction.Down  => 0,
            Direction.Left  => 90,
            Direction.Right => 270,
            _               => 0,
        };

    public override void Dispose() {
        GC.SuppressFinalize(this);
        ((IDestructible)this).UnregisterDestructible();
        base.Dispose();
    }
}