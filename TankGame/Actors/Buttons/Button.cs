using System;
using System.Collections.Generic;
using SFML.System;
using SFML.Window;
using TankGame.Actors.Data;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Buttons;

public abstract class Button : Actor, IClickable {
    protected Button(Vector2f position, Vector2f size) : base(position, size) {
        BoundingBox = new(Position, Size);

        (this as IClickable).RegisterClickable();

        RenderLayer = RenderLayer.MenuFront;
        RenderView = RenderView.Menu;
    }

    protected RectangleComponent BoundingBox { get; init; }
    public override HashSet<IRenderComponent> RenderComponents => new() { BoundingBox };

    public abstract bool OnClick(MouseButtonEventArgs eventArgs);

    public override void Dispose() {
        base.Dispose();
        GC.SuppressFinalize(this);
        (this as IClickable).UnregisterClickable();
    }
}