﻿using System.Diagnostics.CodeAnalysis;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Core.Textures;

namespace TankGame.Actors.GameObjects.Buildings.Towers; 

[SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")]
public class CompletedTower : GameObject {
    public new class Dto : GameObject.Dto { }
    
    private static readonly Texture Tex = TextureManager.Get(TextureType.GameObject, "towerdestroyed");
    public CompletedTower(Vector2i coords) : base(coords, Tex) { }
    public CompletedTower(Dto dto, Vector2i coords) : base(dto, Tex, coords) { }
    public override DestructabilityType DestructabilityType => DestructabilityType.Indestructible;
    public override bool StopsProjectile => true;
    public override float SpeedModifier => 0;
    public override bool Traversible => false;
    protected override Option<GameObject> AfterDestruction => None;
}