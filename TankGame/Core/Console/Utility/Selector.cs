using System.Linq;
using LanguageExt;
using SFML.System;
using TankGame.Actors;
using TankGame.Actors.Fields;
using TankGame.Extensions;

namespace TankGame.Core.Console.Utility; 

public static class Selector {
    public static SelectedData Select(string input) {
        return Gamestates.Gamestate.Player.Match<SelectedData>(
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
        => SelectedData.From(Gamestates.Gamestate.Player);

    private static SelectedData SelectAllEnemies()
        => new(toSet<Actor>(Gamestates.Gamestate.Level.Regions.Values.Bind(region => region.Enemies)));

    private static SelectedData SelectClosestEnemy(Vector2i coords)
        => new(Optional<Actor>(Gamestates.Gamestate.Level.Regions.Values.Bind(region => region.Enemies).MinBy(e => e.Coords.EuclideanDistance(coords))));
    
    private static SelectedData SelectAllGameObjects()
        => new(toSet<Actor>(
            Gamestates.Gamestate
                      .Level.Regions.Values
                      .Bind(region => region.Fields.Values)
                      .Map(field => field.GameObject)
                      .Somes())
        );

    private static SelectedData SelectClosestGameObject(Vector2i coords)
        => new(Optional<Actor>(Gamestates.Gamestate
                                         .Level.Regions
                                         .Values
                                         .Bind(region => region.Fields.Values)
                                         .Map(field => field.GameObject)
                                         .Somes()
                                         .MinBy(gameObject => gameObject.Coords.EuclideanDistance(coords)))
        );

    private static SelectedData SelectAtCoords(Vector2i coords)
        => Gamestates.Gamestate
                     .Level
                     .FieldAt(coords)
                     .Match(ExtractActorsFromField, () => new());

    private static SelectedData ExtractActorsFromField(Field f)
        => new(toSet(List(f, f.Pawn.Map(Actor.ToActor), f.GameObject.Map(Actor.ToActor)).Somes()));
}