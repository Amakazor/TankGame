using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using LanguageExt;
using SFML.System;
using TankGame.Actors;
using TankGame.Actors.Borders;
using TankGame.Actors.Fields;
using TankGame.Actors.GameObjects.Buildings.Activities;
using TankGame.Actors.GameObjects.Buildings.Utility;
using TankGame.Actors.Pawns;
using TankGame.Actors.Pawns.Enemies;
using TankGame.Actors.Pawns.Players;
using TankGame.Core.Textures;
using TankGame.Events;
using TankGame.Pathfinding;

namespace TankGame.Core.Map;

public class Region : IDisposable {
    public class Dto {
        public Vector2i Coords { get; set; }
        public Dictionary<Vector2i, Field.Dto> Fields { get; set; }
        public IEnumerable<Enemy.Dto> Enemies { get; set; }
        public Activity.Dto Activity { get; set; }

        public Player.Dto? Player { get; set; }
        [JsonIgnore] public Option<Player.Dto> PlayerOption {
            get => Player;
            set => Player = value.MatchUnsafe(player => player, () => null);
        }
    }
    public Region(Vector2i coords, Seq<Field> fields, System.Collections.Generic.HashSet<Enemy> enemies, Option<Player> player, Activity activity) {
        Coords = coords;
        Fields = fields.ToDictionary(field => field.Coords, field => field);
        Enemies = enemies;
        Enemies = new();
        Activity = activity;

        MessageBus.PawnDeath += OnPawnDeath;

        RegionBorder = GenerateBorder(coords);

        player.IfSome(p => {
            Gamestates.Gamestate.Player = p;
            Player = p;
            MessageBus.PlayerHealthChanged.Invoke(p.Health);
            if (p.Health == 0) p.Destroy();

            FieldAt(p.Coords)
               .IfSome(field => field.Pawn = p);
        });
    }
    
    public Region(Dto dto) {
        Coords = dto.Coords;
        Fields = dto.Fields.ToDictionary(field => field.Key, field => FieldFactory.Create(field.Value, field.Key));
        Enemies = dto.Enemies.Map(enemy => EnemyFactory.CreateEnemy(enemy, this)).Somes().ToHashSet();
        Activity = ActivityFactory.Create(dto.Activity, this);
        Player = dto.PlayerOption.Map(player => new Player(player));

        MessageBus.PawnDeath += OnPawnDeath;

        RegionBorder = GenerateBorder(dto.Coords);

        Player.IfSome(p => {
            Gamestates.Gamestate.Player = p;
            MessageBus.PlayerHealthChanged.Invoke(p.Health);
            if (p.Health == 0) p.Destroy();

            FieldAt(p.Coords).IfSome(field => field.Pawn = p);
        });
    }

    private static RegionBorder GenerateBorder(Vector2i coords)
        => new((Vector2f)((coords * 64) * ((Level.FieldsInLine + 64) * (Level.FieldsInLine / 2))), new Vector2f(1f, 1f) * 64 * Level.FieldsInLine, TextureManager.Get(TextureType.Border, "region"));

    public Vector2i Coords { get; set; }
    public Dictionary<Vector2i, Field> Fields { get; set; }
    public System.Collections.Generic.HashSet<Enemy> Enemies { get; }
    public Option<Player> Player { get; set; }
    [JsonIgnore] private RegionBorder RegionBorder { get; }
    public Activity Activity { get; set; }

    [JsonIgnore] public bool HasDestructibleActivity => Activity is { DestructabilityType: DestructabilityType.Destructible, ActivityStatus: ActivityStatus.Started };

    public void Dispose() {
        GC.SuppressFinalize(this);
        foreach ((_, Field field) in Fields) field.Dispose();
        Fields.Clear();

        Enemies.ToList()
               .ForEach(enemy => enemy?.Dispose());
        Enemies.Clear();

        Activity.Dispose();
        Player.IfSome(player => player.Dispose());

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
        if (sender is Enemy enemy && HasField(enemy.Coords)) DeleteEnemy(enemy);
    }
    
    public bool HasField(Vector2i coords)
        => Fields.ContainsKey(coords);
    
    public Option<Field> FieldAt(Vector2i coords)
        => Fields.TryGetValue(Key: coords);

    public ISet<Node> GetNodesInRegion() {
        return Fields
              .Values
              .Select(field => new Node(field.Coords, field.Traversible, field.SpeedModifier))
              .ToHashSet();
    }

    public void DeletePlayer() {
        Player = null;
        if (Activity.ActivityStatus == ActivityStatus.Started) Activity.ChangeStatus(ActivityStatus.Stopped);
    }

    public void AddPlayer(Player player) {
        Player = player;
        if (Activity.ActivityStatus == ActivityStatus.Stopped) Activity.ChangeStatus(ActivityStatus.Started);
    }

    public void DeleteEnemy(Enemy enemy) {
        Enemies.Remove(enemy);
        Level map = Gamestates.Gamestate.Level;

        map.FieldAt(enemy.Coords)    .IfSome(field => field.Pawn.IfSome(pawn => { if (pawn == enemy) field.Pawn = null; }));
        map.FieldAt(enemy.LastCoords).IfSome(field => field.Pawn.IfSome(pawn => { if (pawn == enemy) field.Pawn = null; }));
    }

    public void AddEnemy(Enemy enemy)
        => Enemies.Add(enemy);
    
    public bool IsNextTo(Vector2i coords)
        => Math.Abs(Coords.X - coords.X) <= 1 && Math.Abs(Coords.Y - coords.Y) <= 1;

    public Dto ToDto() {
        Dictionary<Vector2i, Field.Dto> fields = new(Fields.Map(pair => new KeyValuePair<Vector2i, Field.Dto>(pair.Key, pair.Value.ToDto())));
        var enemies = Enemies.Map(enemy => enemy.ToDto());
        return new() {
            Coords = Coords,
            Fields = fields,
            Enemies = enemies,
            PlayerOption = Player.Map(player => player.ToDto()),
            Activity = Activity.ToDto(),
        };
    }
    
    public void PostProcess() {
        foreach ((_, Field field) in Fields) field.PostProcess();
    }
}