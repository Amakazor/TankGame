using System;
using System.Collections.Generic;
using System.Text;

namespace TankGame.Src.Events
{
    class PlayerHealthChangeEventArgs : EventArgs
    {
        public int CurrentHealth { get; }

        public PlayerHealthChangeEventArgs(int currentHealth)
        {
            CurrentHealth = currentHealth;
        }
    }
}
