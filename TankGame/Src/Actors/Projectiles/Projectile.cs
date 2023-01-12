﻿using System;
using System.Collections.Generic;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Actors.Pawns;
using TankGame.Actors.Pawns.Enemies;
using TankGame.Actors.Pawns.Player;
using TankGame.Core.Gamestate;
using TankGame.Core.Map;
using TankGame.Core.Sounds;
using TankGame.Core.Textures;
using TankGame.Events;
using TankGame.Extensions;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Projectiles;

public class Projectile : TickableActor {
    private const float BaseFlightDistance = 64 * 6.5F;
    private const float BaseSpeed = 200;

    private readonly float FlightDistance = BaseFlightDistance * (1 / (GamestateManager.WeatherModifier * GamestateManager.WeatherModifier));

    private readonly float FlightSpeed = BaseSpeed * (1 / (GamestateManager.WeatherModifier * GamestateManager.WeatherModifier));

    private Projectile(Vector2f position, Direction direction, Pawn owner) : base(position, new(64, 64)) {
        StartingPosition = Position;
        Direction = direction;
        Owner = owner;

        ProjectileComponent = Owner switch {
            Enemy _  => new(Position, Size, TextureManager.GetTexture(TextureType.Projectile, "pocisk1"), new(255, 255, 255, 255), Direction),
            Player _ => new(Position, Size, TextureManager.GetTexture(TextureType.Projectile, "pocisk2"), new(255, 255, 255, 255), Direction),
            _        => throw new NotImplementedException(),
        };

        MessageBus.RegisterProjectile.Invoke(this);
        SoundManager.PlayRandomSound("shot", Position / 64);

        RenderLayer = RenderLayer.Projectile;
        RenderView = RenderView.Game;
    }

    private Direction Direction { get; }
    private SpriteComponent ProjectileComponent { get; }
    public Pawn Owner { get; private set; }
    private Vector2f StartingPosition { get; }
    private bool HasFlownToFar => StartingPosition.ManhattanDistance(Position) >= FlightDistance;

    private float SpeedMultiplier => (float)(-(Math.Pow(Math.Cos(Math.PI * FlightProgressReversed), FlightProgressReversed > 0.5 ? 3 : 9) - 1) / 2 + 0.2);

    private double FlightProgressReversed => 1    - StartingPosition.ManhattanDistance(Position) / FlightDistance;
    public Vector2f CollisionPosition => Position + (Size - CollistionSize)                      / 2;
    public Vector2f CollistionSize => Size / 4;

    public override HashSet<IRenderComponent> RenderComponents => new() { ProjectileComponent };

    public override void Tick(float deltaTime) {
        if (!HasFlownToFar && !(GamestateManager.Map != null && GameMap.IsOutOfBounds(Position))) {
            Vector2f moveVector = Direction switch {
                Direction.Up    => new(0, -FlightSpeed * deltaTime * SpeedMultiplier),
                Direction.Down  => new(0, FlightSpeed  * deltaTime * SpeedMultiplier),
                Direction.Left  => new(-FlightSpeed    * deltaTime * SpeedMultiplier, 0),
                Direction.Right => new(FlightSpeed     * deltaTime * SpeedMultiplier, 0),
                _               => new(0, 0),
            };

            Position += moveVector;
            ProjectileComponent.SetPosition(Position);
        } else { Dispose(); }
    }

    public override void Dispose() {
        Owner = null;
        MessageBus.UnregisterProjectile.Invoke(this);
        base.Dispose();
    }

    public static Projectile CreateProjectile(Vector2f position, Direction direction, Pawn owner)
        => new(position, direction, owner);
}