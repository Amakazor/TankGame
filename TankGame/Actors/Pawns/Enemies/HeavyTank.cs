using SFML.Graphics;
using SFML.System;
using TankGame.Core.Map;
using TankGame.Core.Textures;

namespace TankGame.Actors.Pawns.Enemies; 

public sealed class HeavyTank : Enemy {
    private const int Score = 100;
    private const int Sight = 5;
    private const float Delay = 2;
    private static readonly Texture Tex = TextureManager.Get(TextureType.Pawn, "enemy3");
    
    public new class Dto : Enemy.Dto { }
    public HeavyTank(Vector2i coords) : base(coords, Tex, 1, Score, Sight, Delay) {
        Register();
    }

    public HeavyTank(Dto dto, Region region) : base(dto, Tex, Sight) {
        Register(region);
    }
}