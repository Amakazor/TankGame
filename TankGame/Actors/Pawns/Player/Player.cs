using System;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Actors.Brains;
using TankGame.Actors.Brains.Goals;
using TankGame.Core.Textures;
using TankGame.Events;

namespace TankGame.Actors.Pawns.Player;

public class Player : Pawn {
    [JsonConstructor] public Player(Vector2i coords, Direction direction, int? health = 5) : base(new(coords.X * 64.0F, coords.Y * 64.0F), new(64.0f, 64.0f), TextureManager.Get(TextureType.Pawn, "player1"), health) {
        Health = health;
        Direction = direction;
        RealPosition = Position;
        MessageBus.PlayerMoved.Invoke(this);
        MessageBus.PlayerHealthChanged.Invoke(CurrentHealth);
        
        Brain.AddGoal(1, new BePlayerGoal(Brain));
    }

    public int? Health {
        get => CurrentHealth;
        set => CurrentHealth = value ?? CurrentHealth;
    }


    public void AddHealth(int amount) {
        CurrentHealth = Math.Min(10, CurrentHealth + amount);
        MessageBus.PlayerHealthChanged.Invoke(CurrentHealth);
    }

    public override void OnHit() {
        base.OnHit();
        MessageBus.PlayerHealthChanged.Invoke(CurrentHealth);
    }
}