using System;
using SFML.System;
using TankGame.Actors.Fields.Roads;

namespace TankGame.Actors.Fields; 

public static class FieldFactory {
    public static Field Create(Field.Dto fieldDto, Vector2i coords)
        => fieldDto switch {
            Grass.Dto dto => new Grass(dto, coords),
            Road.Dto dto  => new Road(dto, coords),
            Sand.Dto dto  => new Sand(dto, coords),
            Water.Dto dto => new Water(dto, coords),
            _             => throw new ArgumentOutOfRangeException(nameof(fieldDto), fieldDto, null),
        };
}