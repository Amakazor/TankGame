using LanguageExt;

namespace TankGame.Core.Console.Commands; 

public interface ICommand {
    public static Option<ICommand> Parse(Seq<string> args) {
        return None;
    }

    public static string Name { get; } = "";
    
    public void Execute();
}