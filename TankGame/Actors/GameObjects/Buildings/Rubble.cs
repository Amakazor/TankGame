using System.Diagnostics.CodeAnalysis;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Core.Textures;

namespace TankGame.Actors.GameObjects.Buildings; 

[SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")]
public class Rubble : GameObject {
    public new class Dto : GameObject.Dto{ }
    
    private static readonly Texture Tex = TextureManager.Get(TextureType.GameObject, "Rubble");
    public Rubble(Vector2i coords) : base(coords, Tex) { }
    public Rubble(Dto dto, Vector2i coords) : base(dto, Tex, coords) { }
    public override DestructabilityType DestructabilityType => DestructabilityType.Indestructible;
    public override bool StopsProjectile => false;
    public override float SpeedModifier => 1.2f;
    public override bool Traversible => true;
    protected override Option<GameObject> AfterDestruction => None;
}