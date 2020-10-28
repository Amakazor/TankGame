using SFML.Graphics;
using TankGame.Src.Actors;

namespace TankGame.Src.Gui.RenderComponents
{
    public interface IRenderComponent
    {
        public IRenderable GetActor();
        public Drawable GetShape();
    }
}
