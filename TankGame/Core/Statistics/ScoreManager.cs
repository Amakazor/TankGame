using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TankGame.Core.Statistics;

public static class ScoreManager {
    private const string Path = "Resources/Scores/Scores.json";
    
    private static List<Score> Scores { get; } = Load();

    public static void Add(Score score) {
        Scores.Add(score);
        Save();
    }

    public static IEnumerable<Score> SkipTake(int skip, int take)
        => Scores.Skip(skip)
                 .Take(take);

    public static List<Score> Load()
        => JsonSerializer.Deserialize<List<Score>>(File.ReadAllText(Path)) ?? new List<Score>();

    public static void Save()
        => File.WriteAllText(Path, JsonSerializer.Serialize(Scores));

    public static int GetScoresCount()
        => Scores.Count;
}