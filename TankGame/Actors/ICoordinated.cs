using System.Linq;
using SFML.System;
using TankGame.Core.Gamestate;
using TankGame.Extensions;

namespace TankGame.Actors; 

public interface ICoordinated {
    public Vector2i Coords { get; set; }
    
    public bool HasClearLineOfSightTo(ICoordinated other)
        => !Coords.GetAllVectorsBetween(other.Coords)
                  .Select(coords => GamestateManager.Map.GetFieldFromRegion(coords))
                  .Any(field => field.Match(f => f.CanBeShootThrough(true), false));
}