using LanguageExt;

namespace TankGame.Core.Console.Commands; 

public static class CommandFactory {
    public static Option<ICommand> CreateCommand(Seq<string> args) {
        
        if (!args.Any()) return null;
        string name = args.First();
        
        return name switch {
            ExitCommand    .Name => ExitCommand    .Parse(args),
            RestartCommand .Name => RestartCommand .Parse(args),
            TeleportCommand.Name => TeleportCommand.Parse(args),
            SpawnCommand   .Name => SpawnCommand   .Parse(args),
            KillCommand    .Name => KillCommand    .Parse(args),
            _                    => null,
        };
    }
}