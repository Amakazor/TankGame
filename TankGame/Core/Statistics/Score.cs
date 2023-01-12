namespace TankGame.Core.Statistics;

public class Score {
    public Score(string name, long points) {
        Name = name;
        Points = points;
    }

    public string Name { get; }
    public long Points { get; }
}