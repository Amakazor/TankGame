#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Actors.Borders;
using TankGame.Actors.Fields;
using TankGame.Actors.GameObjects.Activities;
using TankGame.Actors.Pawns;
using TankGame.Actors.Pawns.Enemies;
using TankGame.Actors.Pawns.Player;
using TankGame.Core.Gamestate;
using TankGame.Core.Textures;
using TankGame.Events;
using TankGame.Extensions;
using TankGame.Pathfinding;

namespace TankGame.Core.Map;

public class Region : IDisposable {
    [JsonConstructor] public Region(Vector2i coords, int fieldsInLine, List<Field> fields, HashSet<Enemy> enemies, Player? player, Activity activity) {
        Coords = coords;
        FieldsInLine = fieldsInLine;
        Fields = fields;
        Enemies = enemies;
        Activity = activity;
        Activity.Region = this;

        MessageBus.PawnDeath += OnPawnDeath;

        RegionBorder = new(new(coords.X * 64 * fieldsInLine + 64 * (fieldsInLine / 2) - 32, coords.Y * 64 * fieldsInLine + 64 * (fieldsInLine / 2) - 32), new(64 * fieldsInLine, 64 * fieldsInLine), TextureManager.Get(TextureType.Border, "region"));

        AddPlayer(player);
        if (Player is null) return;

        MessageBus.PlayerHealthChanged.Invoke(Player.CurrentHealth);
        if (Player.CurrentHealth == 0) Player.OnDestroy();

        GetFieldAtMapCoords(Player.Coords)!.PawnOnField = Player;
    }

    public Vector2i Coords { get; set; }
    public int FieldsInLine { get; }
    public List<Field> Fields { get; set; }
    public HashSet<Enemy> Enemies { get; }
    public Player? Player { get; set; }
    [JsonIgnore] private RegionBorder RegionBorder { get; }
    public Activity Activity { get; set; }

    [JsonIgnore] public bool HasDestructibleActivity => Activity is { IsDestructible: true, ActivityStatus: ActivityStatus.Started };

    public void Dispose() {
        GC.SuppressFinalize(this);
        Fields.ForEach(field => field.Dispose());
        Fields.Clear();

        Enemies.ToList()
               .ForEach(enemy => enemy?.Dispose());
        Enemies.Clear();

        Activity.Dispose();
        Player?.Dispose();

        MessageBus.PawnDeath -= OnPawnDeath;
    }

    public void Save() {
        var serializerOptions = new JsonSerializerOptions {
            WriteIndented = true, IncludeFields = true, NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        string serializedRegion = JsonSerializer.Serialize(this, serializerOptions);

        File.WriteAllText(RegionPathGenerator.GetSavedRegionPath(Coords), serializedRegion);
    }

    private void OnPawnDeath(Pawn sender) {
        if (sender is not Enemy enemy || GetFieldAtMapCoords(enemy.Coords) == null) return;

        DeleteEnemy(enemy);
    }

    public bool HasField(Vector2i mapFieldCoords)
        => Coords.X * FieldsInLine <= mapFieldCoords.X && Coords.Y * FieldsInLine <= mapFieldCoords.Y && (Coords.X + 1) * FieldsInLine > mapFieldCoords.X && (Coords.Y + 1) * FieldsInLine > mapFieldCoords.Y;

    public Field? GetFieldAtMapCoords(Vector2i mapFieldCoords)
        => HasField(mapFieldCoords) ? Fields[ConvertRegionFieldCoordsToFieldIndex(ConvertMapCoordsToRegionFieldCoords(mapFieldCoords))] : null;

    public Vector2i ConvertMapCoordsToRegionFieldCoords(Vector2i mapFieldCoords)
        => mapFieldCoords.Modulo(FieldsInLine);

    private int ConvertRegionFieldCoordsToFieldIndex(Vector2i regionFieldCoords)
        => regionFieldCoords.X * FieldsInLine + regionFieldCoords.Y;

    public ISet<Node> GetNodesInRegion() {
        return Fields
              .Select(field => new Node(field.Coords, field.IsTraversible(true), field.TraversabilityMultiplier))
              .ToHashSet();
    }

    public void DeletePlayer() {
        Player = null;
        if (Activity.ActivityStatus == ActivityStatus.Started) Activity.ChangeStatus(ActivityStatus.Stopped);
    }

    public void AddPlayer(Player? player) {
        if (player is null) return;

        GamestateManager.Player = player;
        Player = player;
        if (Activity.ActivityStatus == ActivityStatus.Stopped) Activity.ChangeStatus(ActivityStatus.Started);
    }

    public void DeleteEnemy(Enemy enemy) {
        Enemies.Remove(enemy);
        GameMap map = GamestateManager.Map;

        if (map.GetFieldFromRegion(enemy.Coords)
              ?.PawnOnField == enemy)
            map.GetFieldFromRegion(enemy.Coords)!.PawnOnField = null;
        if (map.GetFieldFromRegion(enemy.LastCoords)
              ?.PawnOnField == enemy)
            map.GetFieldFromRegion(enemy.LastCoords)!.PawnOnField = null;
    }

    public void AddEnemy(Enemy enemy)
        => Enemies.Add(enemy);
}