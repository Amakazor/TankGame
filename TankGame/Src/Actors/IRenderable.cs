using System.Collections.Generic;
using TankGame.Src.Actors.Data;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors
{
    internal interface IRenderable
    {
        public bool Visible { get; set; }
        public RenderLayer RenderableRenderLayer { get; }
        public RenderView RenderableRenderView { get; }

        HashSet<IRenderComponent> GetRenderComponents();
        public void RegisterRenderable();
        public void UnregisterRenderable();
    }
}