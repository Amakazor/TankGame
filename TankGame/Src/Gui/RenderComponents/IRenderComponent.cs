using SFML.Graphics;
using SFML.System;

namespace TankGame.Gui.RenderComponents;

public interface IRenderComponent {
    public Drawable Shape { get; }

    public bool IsPointInside(Vector2f point);

    public void SetPosition(Vector2f position);

    public void SetSize(Vector2f size);
}