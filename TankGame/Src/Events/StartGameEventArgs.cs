using System;

namespace TankGame.Src.Events
{
    class StartGameEventArgs : EventArgs
    {
        public bool NewGame { get; }

        public StartGameEventArgs(bool newGame)
        {
            NewGame = newGame;
        }
    }
}
