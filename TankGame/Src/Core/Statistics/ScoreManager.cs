using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TankGame.Core.Statistics;

public static class ScoreManager {
    private const string ScoresPath = "Resources/Scores/Scores.json";

    static ScoreManager()
        => Scores = Load();

    private static List<Score> Scores { get; }

    public static void Add(Score score) {
        Scores.Add(score);
        Save();
    }

    public static IEnumerable<Score> GetChunk(int count, int offset)
        => Scores.Skip(offset)
                 .Take(count);

    public static List<Score> Load()
        => JsonSerializer.Deserialize<List<Score>>(File.ReadAllText(ScoresPath)) ?? new List<Score>();

    public static void Save()
        => File.WriteAllText(ScoresPath, JsonSerializer.Serialize(Scores));

    public static int GetScoresCount()
        => Scores.Count;
}