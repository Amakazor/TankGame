using System.Linq;
using LanguageExt;
using TankGame.Actors;

namespace TankGame.Core.Console.Utility; 

public class SelectedData {
    
    public SelectedData(Set<Actor> actors)
        => Actors = actors;
    
    public SelectedData(Actor actor)
        => Actors = Set(actor);
    
    public SelectedData(Option<Actor> actor)
        => Actors = actor.Match(a => Set(a), Set<Actor>());

    public SelectedData()
        => Actors = new();

    public static SelectedData From<T>(Option<T> actor) where T : Actor
        => new(actor.Map(Actor.ToActor));

    private Set<Actor> Actors { get; }

    public Option<T> Get<T>()
        => Actors.Filter(actor => actor is T).Cast<T>().FirstOrDefault();

    public Set<T> GetAll<T>()
        => toSet(Actors.Filter(actor => actor is T).Cast<T>());
}