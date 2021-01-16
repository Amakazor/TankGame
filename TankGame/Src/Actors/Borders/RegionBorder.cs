using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using TankGame.Src.Gui.RenderComponents;
using TankGame.Src.Actors.Data;


namespace TankGame.Src.Actors.Borders
{
    class RegionBorder : Actor
    {
        protected SpriteComponent BorderComponent { get; set; }

        public RegionBorder(Vector2f position, Vector2f size, Texture borderTexture) : base(position, size)
        {
            borderTexture.Repeated = true;

            BorderComponent = new SpriteComponent(position, size, borderTexture, new Color(255, 255, 255, 255));
            BorderComponent.SetScale(new Vector2f(1, 1));

            RenderLayer = RenderLayer.RegionBorder;
            RenderView = RenderView.Game;
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { BorderComponent };
        }

        public override void Dispose()
        {
            BorderComponent = null;
            base.Dispose();
        }
    }
}
