using System;
using TankGame.Src.Actors.GUI;

namespace TankGame.Src.Data.GUI
{
    internal class Hud : IDisposable
    {
        private HealthDisplay HealthDisplay { get; }
        private ActivityDisplay ActivityDisplay { get; }

        public Hud()
        {
            HealthDisplay = new HealthDisplay();
            ActivityDisplay = new ActivityDisplay();
        }

        public void Dispose()
        {
            HealthDisplay.Dispose();
            ActivityDisplay.Dispose();
        }
    }
}