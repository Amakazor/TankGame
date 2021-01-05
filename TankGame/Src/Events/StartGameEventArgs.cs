using System;
using System.Collections.Generic;
using System.Text;

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
