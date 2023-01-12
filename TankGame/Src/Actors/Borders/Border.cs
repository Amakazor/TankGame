using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Borders;

public class Border : Actor {
    public Border(Vector2f position, Vector2f size, Vector2i sizeMultiplier, Texture borderTexture) : base(position, size) {
        borderTexture.Repeated = true;

        BorderComponent = new(position, size, borderTexture, new(255, 255, 255, 255));
        BorderComponent.SetTextureRectSize(new((int)Size.X * sizeMultiplier.X, (int)Size.Y * sizeMultiplier.Y));

        RenderLayer = RenderLayer.Border;
        RenderView = RenderView.Game;
    }

    protected SpriteComponent BorderComponent { get; }

    public override HashSet<IRenderComponent> RenderComponents => new() { BorderComponent };
}