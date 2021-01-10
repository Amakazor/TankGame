using System;
using TankGame.Src.Actors.Pawns;

namespace TankGame.Src.Events
{
    internal class PawnEventArgs : EventArgs
    {
        public Pawn Pawn { get; }

        public PawnEventArgs(Pawn pawn)
        {
            Pawn = pawn;
        }
    }
}