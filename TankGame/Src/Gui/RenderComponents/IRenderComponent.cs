using SFML.Graphics;
using SFML.System;
using TankGame.Src.Actors;

namespace TankGame.Src.Gui.RenderComponents
{
    internal interface IRenderComponent
    {
        public IRenderable Actor { get; }

        public Drawable Shape { get; }

        public bool IsPointInside(Vector2f point);

        public void SetPosition(Vector2f position);

        public void SetSize(Vector2f size);
    }
}