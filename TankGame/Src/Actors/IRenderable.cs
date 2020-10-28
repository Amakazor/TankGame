using System.Collections.Generic;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors
{
    public interface IRenderable
    {
        List<IRenderComponent> GetRenderObjects();
        public void RegisterRenderable(IRenderable renderable);
        public void UnregisterRenderable(IRenderable renderable);
    }
}
