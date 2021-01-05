using System.Collections.Generic;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors
{
    internal interface IRenderable
    {
        HashSet<IRenderComponent> GetRenderComponents();

        public void RegisterRenderable();

        public void UnregisterRenderable();

        public RenderLayer RenderableRenderLayer { get;}
        public RenderView RenderableRenderView { get;}
    }
}