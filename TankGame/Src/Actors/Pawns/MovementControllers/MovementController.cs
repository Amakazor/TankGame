using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Actors.Fields;
using TankGame.Actors.Pawns.Enemies;
using TankGame.Actors.Projectiles;
using TankGame.Core.Controls;
using TankGame.Core.Gamestate;
using TankGame.Core.Map;
using TankGame.Extensions;

namespace TankGame.Actors.Pawns.MovementControllers;

//[JsonDerivedType(typeof(AiMovementController), typeDiscriminator: "ai")]
[JsonDerivedType(typeof(PatrolAIMovementController), "Patrol"), JsonDerivedType(typeof(ChaseAIMovementController), "Chase"), JsonDerivedType(typeof(RandomAiMovementController), "Random"), JsonDerivedType(typeof(PlayerMovementController), "Player"), JsonDerivedType(typeof(StandGroundAiMovementController), "Stand")]
public abstract class MovementController {
    [JsonIgnore] protected const double RotationMultiplier = 0.5;

    protected MovementController(double delay, Pawn owner) {
        Delay = delay;
        Owner = owner;
    }

    protected MovementController(double delay)
        => Delay = delay;

    public double Delay { get; }
    [JsonIgnore] protected double Cooldown { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] public Action NextAction { get; set; }

    [JsonIgnore] protected Pawn Owner { get; set; }
    [JsonIgnore] public bool IsRotating { get; protected set; }
    [JsonIgnore] public bool IsMoving { get; protected set; }
    public double RotationCooldown { get; protected set; }
    public double MovementCooldown { get; protected set; }
    [JsonIgnore] public double RotationProgress => 1 - Cooldown / RotationCooldown;
    [JsonIgnore] public double MovementProgress => 1 - Cooldown / MovementCooldown;

    public virtual Direction DoAction(Direction currentDirection) {
        if (CanDoAction()                && NextAction != Action.Nothing) return NextAction is Action.Fire ? Shoot(currentDirection) : MoveOrRotate(currentDirection);
        if (NextAction is Action.Nothing && Owner is Enemy) SetRandomizedCooldown();
        return currentDirection;
    }

    public Direction MoveOrRotate(Direction currentDirection) {
        Direction nextDirection;

        if (NextAction == Action.MoveDown)
            nextDirection = Direction.Down;
        else if (NextAction == Action.MoveUp)
            nextDirection = Direction.Up;
        else if (NextAction == Action.MoveLeft)
            nextDirection = Direction.Left;
        else if (NextAction == Action.MoveRight)
            nextDirection = Direction.Right;
        else
            return currentDirection;

        return nextDirection == currentDirection ? Move(currentDirection, nextDirection) : Rotate(currentDirection, nextDirection);
    }

    public Direction Move(Direction currentDirection, Direction nextDirection) {
        if (Owner is null) return currentDirection;

        Vector2i nextCoords = GetNextCoordsFromDirection(nextDirection, Owner.Coords);
        ClearAction();

        if (!nextCoords.IsValid()) return currentDirection;

        GameMap gameMap = GamestateManager.Map;
        Field nextField = gameMap.GetFieldFromRegion(nextCoords);
        Field prevField = gameMap.GetFieldFromRegion(Owner.Coords);

        if (nextField != null && prevField != null && nextField.IsTraversible()) {
            Owner.Coords = nextCoords;

            if (nextField.GameObject != null && !nextField.GameObject.IsTraversible) return nextDirection;

            IsMoving = true;
            float averageTraversabilityMultiplier = (nextField.TraversabilityMultiplier + prevField.TraversabilityMultiplier) / 2;
            MovementCooldown = averageTraversabilityMultiplier * Delay * GamestateManager.WeatherModifier;
            SetCooldown(averageTraversabilityMultiplier);

            nextField.EnterField(Owner);
        } else { SetRandomizedCooldown(); }

        return nextDirection;
    }

    protected Vector2i GetNextCoordsFromDirection(Direction nextDirection, Vector2i currentCoords)
        => currentCoords + GetDirectionVector(nextDirection);

    private Vector2i GetDirectionVector(Direction direction)
        => direction switch {
            Direction.Down  => new(0, 1),
            Direction.Up    => new(0, -1),
            Direction.Left  => new(-1, 0),
            Direction.Right => new(1, 0),
            _               => new(-1, -1),
        };

    public Direction Shoot(Direction direction) {
        if (Owner is null) return direction;

        SetCooldown();
        ClearAction();
        Projectile.CreateProjectile(
            Owner.Position + GetDirectionVector(direction)
               .ToVector2f() * 32, direction, Owner
        );
        return direction;
    }

    public Direction Rotate(Direction currentDirection, Direction direction) {
        IsRotating = true;

        double newRotationMultiplier = RotationMultiplier * (IsDirectionOpposite(currentDirection, direction) ? 2 : 1);

        RotationCooldown = newRotationMultiplier * Delay * GamestateManager.WeatherModifier;
        SetCooldown(newRotationMultiplier);

        return direction;
    }

    public virtual bool CanDoAction() {
        if (Owner is null) return false;

        bool canDoAction = Cooldown == 0 && Owner.IsAlive;
        if (canDoAction) ClearStatus();
        return canDoAction;
    }

    public void ClearStatus() {
        if (Owner is null) return;

        IsRotating = false;
        IsMoving = false;
        RotationCooldown = 0;
        MovementCooldown = 0;
        Owner.PreviousDirection = Owner.Direction;
        Owner.PreviousPosition = Owner.Position;
    }

    public void ClearAction()
        => NextAction = Action.Nothing;

    protected void SetRandomizedCooldown()
        => SetCooldown(GamestateManager.Random.NextDouble() / 4);

    protected void SetCooldown(double multiplier = 1)
        => Cooldown = Cooldown > 0 ? Cooldown : Delay * multiplier * GamestateManager.WeatherModifier;

    public void Tick(float deltaTime) {
        if (Owner is null) return;

        if (Cooldown > 0) Cooldown -= deltaTime;
        if (Cooldown < 0) Cooldown = 0;

        GameMap gameMap = GamestateManager.Map;

        if (IsMoving && MovementProgress >= 0.8 && gameMap.GetFieldFromRegion(Owner.LastCoords)
                                                         ?.PawnOnField == Owner)
            gameMap.GetFieldFromRegion(Owner.LastCoords)
                   .PawnOnField = null;
    }

    public void AttachOwner(Pawn owner)
        => Owner = owner;

    private static bool IsDirectionOpposite(Direction lastDirection, Direction currentDirection)
        => (lastDirection == Direction.Up && currentDirection == Direction.Down) || (lastDirection == Direction.Down && currentDirection == Direction.Up) || (lastDirection == Direction.Left && currentDirection == Direction.Right) || (lastDirection == Direction.Right && currentDirection == Direction.Left);
}