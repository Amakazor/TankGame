using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.GameObjects;
using TankGame.Core.Textures;

namespace TankGame.Actors.Fields; 

public class Water : Field {
    private static readonly Seq<Texture> Textures = Seq(TextureManager.Get(TextureType.Field, "water"), TextureManager.Get(TextureType.Field, "water2"));

    public new class Dto : Field.Dto { }
    
    public Water(Vector2i coords, Option<GameObject> gameObject) : base(coords, Textures[Random.Next(Textures.Count)], gameObject) { }
    public Water(Dto dto, Vector2i coords) : base(dto, coords, Textures) { }

    public override float BaseSpeedModifier => 0.0f;
    public override bool BaseTraversible => false;
}