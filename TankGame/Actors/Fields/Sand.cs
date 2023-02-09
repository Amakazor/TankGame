using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.GameObjects;
using TankGame.Core.Textures;

namespace TankGame.Actors.Fields; 

public class Sand : Field {
    private static readonly Seq<Texture> Textures = Seq(TextureManager.Get(TextureType.Field, "sand"), TextureManager.Get(TextureType.Field, "sand2"));

    public new class Dto : Field.Dto { }

    public Sand(Vector2i coords, Option<GameObject> gameObject) : base(coords, Textures[Random.Next(Textures.Count)], gameObject) { }
    public Sand(Dto dto, Vector2i coords) : base(dto, coords, Textures) { }

    public override float BaseSpeedModifier => 2.5f;
    public override bool BaseTraversible => true;
}