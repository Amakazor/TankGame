using System;
using TankGame.Actors.GUI;

namespace TankGame.Core.GUI;

public class Hud : IDisposable {
    public Hud() {
        HealthDisplay = new();
        ActivityDisplay = new();
    }

    private HealthDisplay HealthDisplay { get; }
    private ActivityDisplay ActivityDisplay { get; }

    public void Dispose() {
        HealthDisplay.Dispose();
        ActivityDisplay.Dispose();
    }
}