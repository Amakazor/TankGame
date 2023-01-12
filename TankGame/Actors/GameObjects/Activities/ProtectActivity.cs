using System.Text.Json.Serialization;
using SFML.System;

namespace TankGame.Actors.GameObjects.Activities;

public class ProtectActivity : DestroyAllActivity {
    [JsonConstructor] public ProtectActivity(Vector2i coords, string name, string type, int? health, int pointsAdded, ActivityStatus activityStatus, int? enemiesCount) : base(coords, name, type, health ?? 3, pointsAdded, activityStatus, enemiesCount) { }
}