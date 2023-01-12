using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Borders;

internal class RegionBorder : Actor {
    public RegionBorder(Vector2f position, Vector2f size, Texture borderTexture) : base(position, size) {
        borderTexture.Repeated = true;

        BorderComponent = new(position, size, borderTexture, new(255, 255, 255, 255));
        BorderComponent.SetScale(new(1, 1));

        RenderLayer = RenderLayer.RegionBorder;
        RenderView = RenderView.Game;
    }

    protected SpriteComponent BorderComponent { get; set; }

    public override HashSet<IRenderComponent> RenderComponents => new() { BorderComponent };
}