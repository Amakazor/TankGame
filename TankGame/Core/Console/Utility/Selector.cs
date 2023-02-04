using System.Linq;
using LanguageExt;
using SFML.System;
using TankGame.Actors;
using TankGame.Actors.Fields;
using TankGame.Core.Gamestate;
using TankGame.Extensions;

namespace TankGame.Core.Console.Utility; 

public static class Selector {
    public static SelectedData Select(string input) {
        return GamestateManager.Player.Match<SelectedData>(
            player => {
                var data = toArray(input.Split(';'));
                                
                if (data.Length == 0) return new();
                                
                var coords = ConsoleCoordinates.Parse(data[0], data[1], player.Coords);
                if (data.Length >= 2 && coords.IsSome) return SelectAtCoords(coords.IfNone(new Vector2i()));
                                        
                return ApplySelector(data, player.Coords);
            }, () => new());
    }

    private static SelectedData ApplySelector(Arr<string> data, Vector2i coords)
        => data[0] switch {
            "@pc" => SelectPlayer(),
            "@ec" => SelectClosestEnemy(coords),
            "@e"  => SelectAllEnemies(),
            "@gc" => SelectClosestGameObject(coords),
            "@g"  => SelectAllGameObjects(),
            _     => new(),
        };


    private static SelectedData SelectPlayer()
        => SelectedData.From(GamestateManager.Player);

    private static SelectedData SelectAllEnemies()
        => new(toSet<Actor>(GamestateManager.Map.Regions.SelectMany(region => region.Enemies)));

    private static SelectedData SelectClosestEnemy(Vector2i coords)
        => new(Optional<Actor>(GamestateManager.Map.Regions.SelectMany(region => region.Enemies).MinBy(e => e.Coords.EuclideanDistance(coords))));
    
    private static SelectedData SelectAllGameObjects()
        => new(toSet<Actor>(
            GamestateManager
               .Map.Regions
               .SelectMany(region => region.Fields)
               .Select(field => field.GameObject)
               .Somes())
        );

    private static SelectedData SelectClosestGameObject(Vector2i coords)
        => new(Optional<Actor>(GamestateManager
            .Map.Regions
            .SelectMany(region => region.Fields)
            .Select(field => field.GameObject)
            .Somes()
            .MinBy(gameObject => gameObject.Coords.EuclideanDistance(coords)))
        );

    private static SelectedData SelectAtCoords(Vector2i coords)
        => GamestateManager
          .Map
          .GetFieldFromRegion(coords)
          .Match(ExtractActorsFromField, () => new());

    private static SelectedData ExtractActorsFromField(Field f)
        => new(toSet(List(f, f.PawnOnField.Map(Actor.ToActor), f.GameObject.Map(Actor.ToActor)).Somes()));
}