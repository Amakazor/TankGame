using System;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Actors.Pawns.MovementControllers;
using TankGame.Core.Sounds;
using TankGame.Core.Textures;
using TankGame.Events;

namespace TankGame.Actors.Pawns.Player;

public class Player : Pawn {
    [JsonConstructor] public Player(Vector2i coords, MovementController movementController, Direction direction, int? health = 5) : base(new(coords.X * 64.0F, coords.Y * 64.0F), new(64.0f, 64.0f), TextureManager.GetTexture(TextureType.Pawn, "player1"), health) {
        Health = health;
        Direction = direction;
        movementController.AttachOwner(this);
        AttachMovementController(movementController);
        MessageBus.PlayerMoved.Invoke(this);
        MessageBus.PlayerHealthChanged.Invoke(CurrentHealth);
    }

    public int? Health {
        get => CurrentHealth;
        set => CurrentHealth = value ?? CurrentHealth;
    }

    protected override void UpdatePosition(Vector2i lastCoords, Vector2i newCoords) {
        if (lastCoords == newCoords) return;

        SoundManager.PlaySound("move", "Light", Position / 64);
        base.UpdatePosition(lastCoords, newCoords);
        MessageBus.PlayerMoved.Invoke(this);
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