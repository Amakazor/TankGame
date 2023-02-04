using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using TankGame.Actors;
using TankGame.Core.Console.Utility;
using TankGame.Events;

namespace TankGame.Core.Console.Commands; 

public class KillCommand : ICommand {

    public const string Name = "/kill";

    private KillCommand(SelectedData selectedData) {
        SelectedData = selectedData;
    }
    
    private SelectedData SelectedData { get; }

    public static Option<ICommand> Parse(Seq<string> args)
        => args.Length < 2 || args.First() != Name ? None : new KillCommand(Selector.Select(args[1]));

    public void Execute() {
        foreach (IDestructible destructible in SelectedData.GetAll<IDestructible>().Where(destructible => destructible.IsDestructible)) destructible.Destroy();
    }
}