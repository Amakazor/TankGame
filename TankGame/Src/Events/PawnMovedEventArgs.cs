using SFML.System;
using System;

namespace TankGame.Src.Events
{
    internal class PawnMovedEventArgs : EventArgs
    {
        public Vector2i LastCoords { get; }
        public Vector2i NewCoords { get; }

        public PawnMovedEventArgs(Vector2i lastCoords, Vector2i newCoords)
        {
            LastCoords = lastCoords;
            NewCoords = newCoords;
        }
    }
}
