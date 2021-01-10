using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Gui.RenderComponents;
using TankGame.Src.Actors.Data;

namespace TankGame.Src.Actors.Borders
{
    class Border : Actor
    {
        protected SpriteComponent BorderComponent { get; set; }


        public Border(Vector2f position, Vector2f size, Vector2i sizeMultiplier, Texture borderTexture) : base(position, size)
        {
            borderTexture.Repeated = true;

            BorderComponent = new SpriteComponent(position, size, borderTexture, new Color(255, 255, 255, 255));
            BorderComponent.SetTextureRectSize(new Vector2i((int)Size.X * sizeMultiplier.X, (int)Size.Y * sizeMultiplier.Y));

            RenderLayer = RenderLayer.Border;
            RenderView = RenderView.Game;
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { BorderComponent };
        }
    }
}
