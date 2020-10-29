using SFML.Graphics;
using SFML.System;
using TankGame.Src.Actors;

namespace TankGame.Src.Gui.RenderComponents
{
    public interface IRenderComponent
    {
        public IRenderable GetActor();
        public Drawable GetShape();
        public bool IsPointInside(int x, int y);
        public void SetPosition(Vector2f position);
        public void SetSize(Vector2f size);
    }
}
