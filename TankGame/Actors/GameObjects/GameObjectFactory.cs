using System;
using SFML.System;
using TankGame.Actors.GameObjects.Buildings;
using TankGame.Actors.GameObjects.Buildings.Towers;

namespace TankGame.Actors.GameObjects; 

public static class GameObjectFactory {
    public static GameObject Create(GameObject.Dto gameObjectDto, Vector2i coords)
        => gameObjectDto switch {
            BigBuilding.Dto dto    => new BigBuilding(dto, coords),
            ConiferousTree.Dto dto => new ConiferousTree(dto, coords),
            DeciduousTree.Dto dto  => new DeciduousTree(dto, coords),
            FenceBw.Dto dto        => new FenceBw(dto, coords),
            FenceColor.Dto dto     => new FenceColor(dto, coords),
            House.Dto dto          => new House(dto, coords),
            Rubble.Dto dto         => new Rubble(dto, coords),
            Stump.Dto dto          => new Stump(dto, coords),

            CompletedTower.Dto dto => new CompletedTower(dto, coords),
            DestroyedTower.Dto dto => new DestroyedTower(dto, coords),

            _ => throw new ArgumentOutOfRangeException(nameof(gameObjectDto), gameObjectDto, "gameObjectDto is not a known subclass of GameObject.Dto"),
        };
}