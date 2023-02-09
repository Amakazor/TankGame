using System.Linq;
using SFML.System;
using TankGame.Core.Gamestates;
using TankGame.Extensions;

namespace TankGame.Actors; 

public interface ICoordinated {
    public Vector2i Coords { get; set; }
    
    public bool HasClearLineOfSightTo(ICoordinated other)
        => !Coords.GetAllVectorsBetween(other.Coords)
                  .Select(coords => Gamestate.Level.FieldAt(coords))
                  .Any(field => field.Match(f => f.CanBeShootThrough(), false));
}