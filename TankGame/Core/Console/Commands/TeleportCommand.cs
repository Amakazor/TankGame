using LanguageExt;
using SFML.System;
using TankGame.Actors.Fields;
using TankGame.Actors.Pawns.Player;
using TankGame.Core.Console.Utility;
using TankGame.Core.Gamestate;
using TankGame.Events;

namespace TankGame.Core.Console.Commands; 

public class TeleportCommand : ICommand {
    public const string Name = "/tp";
    
    private TeleportCommand(Field field) {
        Field = field;
    }
    
    private Field Field { get; }

    public static Option<ICommand> Parse(Seq<string> args) {
        if (args.HeadOrNone() != Name) return None;
        
        return GamestateManager.Player.Map(player => new ArgsTuple<Player>(args, player))
                               .Bind(GetCoords)
                               .Bind(GetField)
                               .Bind(GetCommand);
    }

    private static Option<ArgsTuple<Vector2i>> GetCoords(ArgsTuple<Player> data)
        => ConsoleCoordinates.Parse(data.Args[1], data.Args[2], data.Value.Coords).Match<Option<ArgsTuple<Vector2i>>>(coordinates => new ArgsTuple<Vector2i>(data.Args, coordinates), None);

    private static Option<ArgsTuple<Field>> GetField(ArgsTuple<Vector2i> data)
        => GamestateManager.Map.GetFieldFromRegion(data.Value).Match<Option<ArgsTuple<Field>>>(field => new ArgsTuple<Field>(data.Args, field), None);

    private static Option<ICommand> GetCommand(ArgsTuple<Field> data)
        => data.Value.CanBeSpawnedOn() ? None : new TeleportCommand(data.Value);

    public void Execute() {
        GamestateManager.Player.IfSome(
            player => {
                 GamestateManager.Map.GetFieldFromRegion(player.Coords).IfSome(field => field.PawnOnField = None);
                Field.DestroyObjectOnEntry(true);
                Field.PawnOnField = player;
                player.SetPosition(new(Field.Coords.X * 64.0f, Field.Coords.Y * 64.0f));
                player.Coords = Field.Coords;
                MessageBus.PlayerMoved(player);
            }
        );
    }
}