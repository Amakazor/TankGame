using System;

namespace TankGame.Src.Events
{
    internal class PlayerHealthChangeEventArgs : EventArgs
    {
        public int CurrentHealth { get; }

        public PlayerHealthChangeEventArgs(int currentHealth)
        {
            CurrentHealth = currentHealth;
        }
    }
}