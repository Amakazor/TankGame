namespace TankGame.Actors.Brains.Thoughts; 

public static class ThoughtFactory {
    public static Thought CreateThought(Brain brain, Thought.Dto thoughtDto) {
        return thoughtDto switch {
            IdleThought.Dto dto   => new IdleThought(brain, dto),
            MoveThought.Dto dto   => new MoveThought(brain, dto),
            RotateThought.Dto dto => new RotateThought(brain, dto),
            ShootThought.Dto dto  => new ShootThought(brain, dto),
        };
    }
}