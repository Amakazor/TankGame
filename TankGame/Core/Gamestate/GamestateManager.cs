using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SFML.System;
using TankGame.Actors.Pawns;
using TankGame.Actors.Pawns.Enemies;
using TankGame.Actors.Pawns.Player;
using TankGame.Actors.Text;
using TankGame.Core.Map;
using TankGame.Core.Weathers;
using TankGame.Events;

namespace TankGame.Core.Gamestate;

public static class GamestateManager {
    private const float ComboTime = 7.50f;
    private const int MaxCombo = 10;
    private const string SavePath = "Resources/Save/Current.json";
    private const int PointsPerHealthUp = 5000;
    private static HashSet<PointsAddedTextBox> _pointsTextBoxes;
    private static readonly Vector2f _pointsMovementVector = new(75, 10);

    static GamestateManager() {
        MessageBus.PawnDeath += OnPawnDeath;
        GamePhase = GamePhase.NotStarted;

        Points = 0;
        Combo = 1;
        CompletedActivities = 0;

        Random = new();

        _pointsTextBoxes = new();

        WeatherController = null;
    }

    public static GamePhase GamePhase { get; private set; }

    public static long Points { get; private set; }
    private static long PointsBeforeSubstraction { get; set; }
    private static int Combo { get; set; }
    private static int CompletedActivities { get; set; }
    private static float ComboDeltaTimeCumulated { get; set; }
    public static GameMap Map { get; private set; }
    public static WeatherController WeatherController { get; set; }
    public static Player? Player { get; set; }
    public static Random Random { get; }
    public static float WeatherModifier => WeatherController.GetSpeedModifier();
    public static float WeatherTime => WeatherController.CurrentWeatherTime;
    public static bool Paused => GamePhase     == GamePhase.Paused;
    public static bool Playing => GamePhase    == GamePhase.Playing;
    public static bool NotStarted => GamePhase == GamePhase.NotStarted;
    public static bool Ending => GamePhase     == GamePhase.Ending;

    public static void Start(bool continueGame) {
        GamePhase = GamePhase.Playing;
        WeatherController = new();

        if (!File.Exists(SavePath) || !continueGame)
            DeleteSave();
        else
            Load();

        Map = new();

        Save();
    }

    private static void OnPawnDeath(Pawn sender) {
        if (sender is not Enemy enemy) return;
        AddPoints(enemy.ScoreAdded, enemy.RealPosition + new Vector2f(enemy.Size.X / 2 - 75, enemy.Size.Y / 10 - 10));
    }

    public static void Pause()
        => GamePhase = GamePhase.Paused;

    public static void Play()
        => GamePhase = GamePhase.Playing;

    public static void End()
        => GamePhase = GamePhase.Ending;

    public static void Save() {
        PrepareSaveFolders();

        File.WriteAllText(SavePath, JsonSerializer.Serialize(new GamestateDto(Points, PointsBeforeSubstraction, Combo, ComboDeltaTimeCumulated, CompletedActivities, WeatherController.WeatherType, WeatherController.CurrentWeatherTime)));

        Map?.Save();
    }

    private static void PrepareSaveFolders() {
        if (!Directory.Exists("Resources/Save")) Directory.CreateDirectory("Resources/Save");
        if (!Directory.Exists("Resources/Save/Region")) Directory.CreateDirectory("Resources/Save/Region");
    }

    private static void Load() {
        var dto = JsonSerializer.Deserialize<GamestateDto>(File.ReadAllText(SavePath));
        if (dto is null) return;

        Points = dto.Points;
        PointsBeforeSubstraction = dto.PointsBeforeSubstraction;
        Combo = dto.Combo;
        ComboDeltaTimeCumulated = dto.ComboDeltaTime;
        CompletedActivities = dto.CompletedActivities;
        WeatherController.SetWeather(dto.WeatherType, dto.RemainingWeatherTime);
    }

    public static void Clear() {
        Map.Dispose();
        _pointsTextBoxes.ToList()
                        .ForEach(pointsTextBox => pointsTextBox.Dispose());
        WeatherController.Dispose();

        Map = null;
        Player = null;
        WeatherController = null;
        _pointsTextBoxes = new();

        GamePhase = GamePhase.NotStarted;
        Points = 0;
        Combo = 1;
        CompletedActivities = 0;
        ComboDeltaTimeCumulated = 0;
    }

    public static void DeleteSave() {
        DirectoryInfo directory = Directory.GetParent(RegionPathGenerator.SavedRegionDirectory);
        if (directory is null || !directory.Exists) return;

        directory.GetFiles()
                 .ToList()
                 .ForEach(fileInfo => fileInfo.Delete());
        Directory.GetParent(directory.ToString())
                ?.GetFiles()
                 .ToList()
                 .ForEach(fileInfo => fileInfo.Delete());
    }

    public static void AddPoints(long points, Vector2f? position = null, bool useCombo = true) {
        position = position is not null ? position + _pointsMovementVector : Player.Position + new Vector2f(Player.Size.X / 2 - 50, Player.Size.Y / 4 - 10);

        if (points < 0) PointsBeforeSubstraction = points;

        long pointsBeforeAddition = Points;
        if (useCombo) {
            ComboDeltaTimeCumulated = 0;
            Points += points * Combo;
            Combo = Math.Min(Combo + 1, MaxCombo);
        } else { Points += points; }

        if (pointsBeforeAddition / PointsPerHealthUp != Points / PointsPerHealthUp && Points > pointsBeforeAddition && Points > PointsBeforeSubstraction) Player.AddHealth(Convert.ToInt32(Points / PointsPerHealthUp - pointsBeforeAddition / PointsPerHealthUp));

        _pointsTextBoxes.Add(new((Vector2f)position, points, useCombo ? Combo - 1 : 1));
    }

    public static void Tick(float deltaTime) {
        foreach (PointsAddedTextBox pointsAddedTextBox in _pointsTextBoxes.ToList()
                                                                          .Where(pointsAddedTextBox => pointsAddedTextBox.TimeToLive < 0))
            _pointsTextBoxes.Remove(pointsAddedTextBox);

        ComboDeltaTimeCumulated += deltaTime;

        if (ComboDeltaTimeCumulated > ComboTime) {
            ComboDeltaTimeCumulated = 0;
            Combo = Math.Max(1, Combo - 1);
        }
    }

    public static void CompleteActivity(int points, Vector2f position)
        => AddPoints((int)(points * (++CompletedActivities / 6F + 5F / 6F)), position, false);

    public static void FailActivity(int points, Vector2f position)
        => AddPoints(-1 * points, position, false);
}