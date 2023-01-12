using System;
using SFML.System;
using TankGame.Actors.Pawns;

namespace TankGame.Events;

public class PawnMovedEventArgs : EventArgs {
    public PawnMovedEventArgs(Vector2i lastCoords, Vector2i newCoords, Pawn pawn) {
        LastCoords = lastCoords;
        NewCoords = newCoords;
        Pawn = pawn;
    }

    public Pawn Pawn { get; }
    public Vector2i LastCoords { get; }
    public Vector2i NewCoords { get; }
}