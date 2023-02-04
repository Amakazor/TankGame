﻿using System;
using LanguageExt;
using SFML.System;
using TankGame.Actors.Pawns;
using TankGame.Core.Gamestate;
using TankGame.Core.Map;

namespace TankGame.Events;

public class PawnMovedEventArgs : EventArgs {
    public PawnMovedEventArgs(Vector2i lastCoords, Vector2i newCoords, Pawn pawn) {
        LastCoords = lastCoords;
        NewCoords = newCoords;
        Pawn = pawn;
    }

    public Option<Tuple<Region, Region>> Regions => 
        !LastCoords.Equals(NewCoords) 
            ? GamestateManager.Map.GetRegionFromFieldCoords(LastCoords).SelectMany(_ => GamestateManager.Map.GetRegionFromFieldCoords(NewCoords), Tuple) 
            : None;

    public Pawn Pawn { get; }
    public Vector2i LastCoords { get; }
    public Vector2i NewCoords { get; }
}