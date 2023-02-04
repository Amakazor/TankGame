using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using TankGame.Events;

namespace TankGame.Core.Console.Commands; 

public class RestartCommand : ICommand {
    public const string Name = "/restart";
    private RestartCommand() {}

    public static Option<ICommand> Parse(Seq<string> args)
        => !args.Any() || args.First() != Name ? None : new RestartCommand();

    public void Execute()
        => MessageBus.StartGame(false);
}