using System;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Brains;
using TankGame.Actors.Brains.Goals;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Core.Controls;
using TankGame.Core.Textures;
using TankGame.Events;

namespace TankGame.Actors.Pawns.Players;

public class Player : Pawn {
    public new class Dto : Pawn.Dto {
        public Dto WithBrain() {
            BrainOption = BrainOption.IfNone(PlayerBrain());
            return this;
        }
    }
    
    private const int Sight = 5;
    private const float Delay = 2;
    private static readonly Texture Tex = TextureManager.Get(TextureType.Pawn, "enemy3");
    
    public Player(Vector2i coords, int health = 5) : base(coords, Tex, health, Sight, Delay) {
        MessageBus.PlayerMoved.Invoke(this);
        MessageBus.PlayerHealthChanged.Invoke(Health);
        
        Brain.AddGoal(1, new BePlayerGoal(Brain));
    }

    public Player(Dto dto) : base(dto.WithBrain(), Tex, Sight) {
        MessageBus.PlayerMoved.Invoke(this);
        MessageBus.PlayerHealthChanged.Invoke(Health);
    }

    public void AddHealth(int amount) {
        Health = Math.Min(10, Health + amount);
        MessageBus.PlayerHealthChanged.Invoke(Health);
    }

    public override void Hit() {
        base.Hit();
        MessageBus.PlayerHealthChanged.Invoke(Health);
    }
    
    private static Brain.Dto PlayerBrain() => new() {
        CurrentThoughtDto = new IdleThought.Dto(),
        Goals = new() {
            {
                1, new BePlayerGoal.Dto {
                NextAction = InputAction.Nothing,
                Id = Guid.NewGuid(),
            }}, {
                2, new IdleGoal.Dto {
                Id = Guid.NewGuid(),
            }},
        },
        Delay = 1.0f,
    };
    
    public override Dto ToDto() => new() {Direction = Direction, Position = Position, PreviousPosition = PreviousPosition, Health = Health, Brain = Brain.ToDto()};
}