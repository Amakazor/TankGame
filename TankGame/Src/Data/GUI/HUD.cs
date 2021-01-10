using System;
using TankGame.Src.Actors.GUI;

namespace TankGame.Src.Data.GUI
{
    class HUD : IDisposable
    {
        private HealthDisplay HealthDisplay { get; }
        private ActivityDisplay ActivityDisplay { get; }
        public HUD()
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
