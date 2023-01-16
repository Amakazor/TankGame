using System.Text.Json.Serialization;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Brains.Goals;

namespace TankGame.Actors.Pawns.Enemies;

public class Enemy : Pawn {
    public Enemy(Vector2f position, Vector2f size, Texture texture, int health, int scoreAdded, EnemyType type) : base(position, size, texture, health) {
        Health = health;
        ScoreAdded = scoreAdded;
        Type = type;

        if (Coords.X != 45 || Coords.Y != 50) {
            OnDestroy();
        }
        
        Brain.AddGoal(1, new ShootPlayerGoal(Brain));
        Brain.AddGoal(2, new ChasePlayerGoal(Brain));
    }

    [JsonConstructor] public Enemy(Vector2i coords, Direction direction, int? health, EnemyType type, int scoreAdded) : base(new(coords.X * 64.0f, coords.Y * 64.0f), new(64.0f, 64.0f), EnemyFactory.GetTexture(type), health) {
        ScoreAdded = scoreAdded;
        Type = type;
        Health = health;
        Direction = direction;
        
        if (Coords.X != 45 || Coords.Y != 50) {
            OnDestroy();
        } else {
            Brain.AddGoal(1, new ShootPlayerGoal(Brain));
            Brain.AddGoal(2, new ChasePlayerGoal(Brain));
        }
    }

    public int ScoreAdded { get; }
    public EnemyType Type { get; }

    public int? Health {
        get => CurrentHealth;
        set => CurrentHealth = value ?? EnemyFactory.DefaultHealth[Type];
    }
}