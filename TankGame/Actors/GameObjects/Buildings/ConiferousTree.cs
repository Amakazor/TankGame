using System.Diagnostics.CodeAnalysis;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Core.Textures;

namespace TankGame.Actors.GameObjects.Buildings; 

[SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")]
public class ConiferousTree : GameObject {
    public new class Dto : GameObject.Dto { }
    
    private static readonly Texture Tex = TextureManager.Get(TextureType.GameObject, "TreeConifer");
    public ConiferousTree(Vector2i coords) : base(coords, Tex, 4) { }
    public ConiferousTree(Dto dto, Vector2i coords) : base(dto, Tex, coords) { }
    public override DestructabilityType DestructabilityType => DestructabilityType.DestroyOnEntry;
    public override bool StopsProjectile => true;
    public override float SpeedModifier => 2f;
    public override bool Traversible => true;
    protected override Option<GameObject> AfterDestruction => new Stump(Coords);
}