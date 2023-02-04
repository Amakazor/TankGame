using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Core.Gamestate;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.GUI;

public class ActivityDisplay : Actor, ITickable {
    public ActivityDisplay() : base(new(10, 64 + 10), new(490, 200 - (64 + 10))) {
        ActivityName = new(Position, new(Size.X, Size.Y                               / 3), new(0, 0), 17, TextPosition.Start, TextPosition.Middle, "", Color.White);
        ActivityText = new(Position + new Vector2f(0, Size.Y / 3), new(Size.X, Size.Y / 3), new(0, 0), 15, TextPosition.Start, TextPosition.Middle, "", Color.White);

        RenderView = RenderView.HUD;
        RenderLayer = RenderLayer.HUDFront;

        (this as ITickable).RegisterTickable();
    }

    private AlignedTextComponent ActivityName { get; }
    private AlignedTextComponent ActivityText { get; }
    public override HashSet<IRenderComponent> RenderComponents => new() { ActivityName, ActivityText };

    public void Tick(float deltaTime)
        => UpdateActivityDisplay();

    private void UpdateActivityDisplay() {
        // if (GamestateManager.Player?.CurrentRegion?.Activity?.ProgressText != null) {
        //     ActivityName.SetText("Current objective: " + GamestateManager.Player.CurrentRegion.Activity.DisplayName);
        //
        //     ActivityText.SetText(GamestateManager.Player.CurrentRegion.Activity.ProgressText ?? "");
        // } else {
        //     ActivityName.SetText("No current objective.");
        //     ActivityText.SetText("");
        // }
    }

    public override void Dispose() {
        GC.SuppressFinalize(this);
        (this as ITickable).UnregisterTickable();
        base.Dispose();
    }
}