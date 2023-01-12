using System;

namespace TankGame.Events;

public class StartGameEventArgs : EventArgs {
    public StartGameEventArgs(bool newGame)
        => NewGame = newGame;

    public bool NewGame { get; }
}