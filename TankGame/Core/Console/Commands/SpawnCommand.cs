using System;
using LanguageExt;
using SFML.System;
using TankGame.Actors.Fields;
using TankGame.Actors.Pawns.Enemies;
using TankGame.Actors.Pawns.Players;
using TankGame.Core.Console.Utility;

namespace TankGame.Core.Console.Commands; 

public class SpawnCommand : ICommand {
    public const string Name = "/spawn";
    
    private SpawnCommand(EnemySpawnData spawnData)
        => SpawnData = spawnData;

    private EnemySpawnData SpawnData { get; }

    public static Option<ICommand> Parse(Seq<string> args) {
        if (args.HeadOrNone() != Name) return None;

        return Gamestates.Gamestate.Player.Map(player => new ArgsTuple<Player>(args, player))
                         .Bind(GetCoords)
                         .Bind(GetField)
                         .Bind(GetCommand);
    }

    private static Option<ArgsTuple<Vector2i>> GetCoords(ArgsTuple<Player> data)
        => ConsoleCoordinates.Parse(data.Args[1], data.Args[2], data.Value.Coords).Match<Option<ArgsTuple<Vector2i>>>(coordinates => new ArgsTuple<Vector2i>(data.Args, coordinates), None);

    private static Option<ArgsTuple<Field>> GetField(ArgsTuple<Vector2i> data)
        => Gamestates.Gamestate.Level.FieldAt(data.Value).Match<Option<ArgsTuple<Field>>>(field => new ArgsTuple<Field>(data.Args, field), None);

    private static Option<ICommand> GetCommand(ArgsTuple<Field> data) {
        if (data.Value.CanBeSpawnedOn()) return None;

        if (data.Args.Count() < 4 || !Enum.TryParse(data.Args[3], true, out EnemyType enemyType)) enemyType = EnemyType.Light;
        return new SpawnCommand(new(data.Value.Coords, enemyType));
    }

    public void Execute()
        => EnemyFactory.CreateEnemy(SpawnData);
}