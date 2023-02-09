using LanguageExt;
using TankGame.Events;

namespace TankGame.Core.Console.Commands;

public class ExitCommand : ICommand {

    public const string Name = "/quit";
    private ExitCommand() {}

    public static Option<ICommand> Parse(Seq<string> args)
        => !args.Any() || args.First() != Name ? None : new ExitCommand();

    public void Execute()
        => MessageBus.Quit();
}