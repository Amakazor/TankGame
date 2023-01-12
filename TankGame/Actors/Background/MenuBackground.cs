using System.Collections.Generic;
using TankGame.Actors.Data;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Background;

public class MenuBackground : Actor {
    public MenuBackground() : base(new(0, 0), new(1000, 1000)) {
        BackgroundRectangle = new(Position, Size, new(128, 128, 128, 170));

        RenderLayer = RenderLayer.MenuBack;
        RenderView = RenderView.Menu;
    }

    private RectangleComponent BackgroundRectangle { get; }

    public override HashSet<IRenderComponent> RenderComponents => new() { BackgroundRectangle };
}