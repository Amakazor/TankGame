using System.Text.Json.Serialization;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Core.Gamestates;
using TankGame.Core.Map;

namespace TankGame.Actors.Pawns.Enemies;

public abstract class Enemy : Pawn {
    [JsonDerivedType(typeof(LightTank.Dto),  typeDiscriminator: nameof(LightTank))]
    [JsonDerivedType(typeof(MediumTank.Dto), typeDiscriminator: nameof(MediumTank))]
    [JsonDerivedType(typeof(HeavyTank.Dto),  typeDiscriminator: nameof(HeavyTank))]
    public new class Dto : Pawn.Dto {
        public int ScoreAdded { get; set; }
    }

    protected Enemy(Vector2i coords, Texture texture, int health, int scoreAdded, int sightDistance, float delay) : base(coords, texture, health, sightDistance, delay) {
        Health = health;
        ScoreAdded = scoreAdded;
    }

    protected Enemy(Dto dto, Texture texture, int sightDistance) : base(dto, texture, sightDistance) {
        ScoreAdded = dto.ScoreAdded;
    }

    public int ScoreAdded { get; }

    protected override void Register(Option<Region> region = default) {
        base.Register(region);
        region
           .Match(reg => reg, () => Gamestate.Level.GetRegionFromFieldCoords(Coords))
           .IfSome(reg => reg.Enemies.Add(this));
    }
    
    public virtual Dto ToDto() => new() {Direction = Direction, Position = Position, PreviousPosition = PreviousPosition, Health = Health, Brain = Brain.ToDto(), ScoreAdded = ScoreAdded};
}