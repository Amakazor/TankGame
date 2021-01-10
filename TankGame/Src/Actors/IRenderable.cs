using System.Collections.Generic;
using TankGame.Src.Actors.Data;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors
{
    internal interface IRenderable
    {
        public bool Visible { get; set; }
        HashSet<IRenderComponent> GetRenderComponents();
        public void RegisterRenderable();
        public void UnregisterRenderable();
        public RenderLayer RenderableRenderLayer { get;}
        public RenderView RenderableRenderView { get;}
    }
}