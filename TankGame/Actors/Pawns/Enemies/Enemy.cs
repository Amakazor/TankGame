using System.Text.Json.Serialization;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Pawns.MovementControllers;
using TankGame.Core.Sounds;

namespace TankGame.Actors.Pawns.Enemies;

public class Enemy : Pawn {
    public Enemy(Vector2f position, Vector2f size, Texture texture, int health, int scoreAdded, EnemyType type) : base(position, size, texture, health) {
        Health = health;
        ScoreAdded = scoreAdded;
        Type = type;
    }

    [JsonConstructor] public Enemy(Vector2i coords, Direction direction, int? health, MovementController movementController, EnemyType type, int scoreAdded) : base(new(coords.X * 64.0f, coords.Y * 64.0f), new(64.0f, 64.0f), EnemyFactory.GetTexture(type), health) {
        ScoreAdded = scoreAdded;
        Type = type;
        Health = health;
        Direction = direction;
        AttachMovementController(movementController);
        movementController.AttachOwner(this);
    }

    public int ScoreAdded { get; }
    public EnemyType Type { get; }

    public int? Health {
        get => CurrentHealth;
        set => CurrentHealth = value ?? EnemyFactory.DefaultHealth[Type];
    }

    protected override void UpdatePosition(Vector2i lastCoords, Vector2i newCoords) {
        if (lastCoords == newCoords) return;

        SoundManager.PlaySound("move", Type.ToString(), Position / 64);
        base.UpdatePosition(lastCoords, newCoords);
    }
}