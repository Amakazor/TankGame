using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.GameObjects;
using TankGame.Core.Textures;
using Seq = LanguageExt.Seq;

namespace TankGame.Actors.Fields; 

public class Grass : Field {
    private static readonly Seq<Texture> Textures = Seq(TextureManager.Get(TextureType.Field, "grass"), TextureManager.Get(TextureType.Field, "grass2"));
    
    public new class Dto : Field.Dto { }

    public Grass(Vector2i coords, Option<GameObject> gameObject) : base(coords, Textures[Random.Next(Textures.Count)], gameObject) { }
    public Grass(Dto dto, Vector2i coords) : base(dto, coords, Textures) { }
    public override float BaseSpeedModifier => 1.2f;
    public override bool BaseTraversible => true;
}