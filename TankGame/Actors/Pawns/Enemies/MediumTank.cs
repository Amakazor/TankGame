using SFML.Graphics;
using SFML.System;
using TankGame.Core.Map;
using TankGame.Core.Textures;

namespace TankGame.Actors.Pawns.Enemies; 

public sealed class MediumTank : Enemy {
    private const int Score = 100;
    private const int Sight = 5;
    private const float Delay = 1.33f;
    private static readonly Texture Tex = TextureManager.Get(TextureType.Pawn, "enemy2");
    
    public new class Dto : Enemy.Dto { }
    public MediumTank(Vector2i coords) : base(coords, Tex, 1, Score, Sight, Delay) {
        Register();
    }

    public MediumTank(Dto dto, Region region) : base(dto, Tex, Sight) {
        Register(region);
    }
}