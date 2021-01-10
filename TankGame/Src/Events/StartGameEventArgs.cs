using System;

namespace TankGame.Src.Events
{
    internal class StartGameEventArgs : EventArgs
    {
        public bool NewGame { get; }

        public StartGameEventArgs(bool newGame)
        {
            NewGame = newGame;
        }
    }
}