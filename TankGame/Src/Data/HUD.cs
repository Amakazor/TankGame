using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Actors.GUI;

namespace TankGame.Src.Data
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
