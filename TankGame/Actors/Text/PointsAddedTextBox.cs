using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Events;

namespace TankGame.Actors.Text;

public class PointsAddedTextBox : TextBox, ITickable {
    public PointsAddedTextBox(Vector2f position, long points, int combo) : base(position, new(100, 20), points + (combo > 1 ? " X " + combo : ""), 16, Color.Red) {
        TimeToLive = 2;
        RegisterTickable();

        RenderLayer = RenderLayer.TextBox;
        RenderView = RenderView.Game;
    }

    public double TimeToLive { get; private set; }

    public void Tick(float deltaTime) {
        TimeToLive -= deltaTime;

        Position += new Vector2f(0, -32) * deltaTime;

        Text.SetPosition(Position);

        if (TimeToLive <= 0) Dispose();
    }

    public void RegisterTickable()
        => MessageBus.RegisterTickable.Invoke(this);

    public void UnregisterTickable()
        => MessageBus.UnregisterTickable.Invoke(this);

    public override void Dispose() {
        UnregisterTickable();
        base.Dispose();
    }
}