using TankGame.Actors.Weathers;

namespace TankGame.Core.Gamestate;

internal class GamestateDto {
    public GamestateDto(long points, long pointsBeforeSubstraction, int combo, float comboDeltaTime, int completedActivities, WeatherType weatherType, float remainingWeatherTime) {
        Points = points;
        PointsBeforeSubstraction = pointsBeforeSubstraction;
        Combo = combo;
        ComboDeltaTime = comboDeltaTime;
        CompletedActivities = completedActivities;
        WeatherType = weatherType;
        RemainingWeatherTime = remainingWeatherTime;
    }

    public long Points { get; }
    public long PointsBeforeSubstraction { get; }
    public int Combo { get; }
    public float ComboDeltaTime { get; }
    public int CompletedActivities { get; }
    public WeatherType WeatherType { get; }
    public float RemainingWeatherTime { get; }
}