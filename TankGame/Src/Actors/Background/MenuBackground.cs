using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Background
{
    internal class MenuBackground : Actor
    {
        private RectangleComponent BackgroundRectangle { get; }

        public MenuBackground() : base(new Vector2f(0, 0), new Vector2f(1000, 1000))
        {
            BackgroundRectangle = new RectangleComponent(Position, Size, new Color(128, 128, 128, 170));

            RenderLayer = RenderLayer.MenuBack;
            RenderView = RenderView.Menu;
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { BackgroundRectangle };
        }
    }
}
