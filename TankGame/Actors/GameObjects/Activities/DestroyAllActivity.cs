using System.Text.Json.Serialization;
using SFML.System;

namespace TankGame.Actors.GameObjects.Activities;

public class DestroyAllActivity : Activity {
    [JsonConstructor] public DestroyAllActivity(Vector2i coords, string name, string type, int? health, int pointsAdded, ActivityStatus activityStatus, int? enemiesCount) : base(coords, name, type, health, pointsAdded, activityStatus, enemiesCount) { }

    protected override string CalculateProgress() {
        if (AllEnemiesCount == 0 && CurrentRegion.Enemies.Count > 0) AllEnemiesCount = CurrentRegion.Enemies.Count;

        if (CurrentRegion.Enemies.Count == 0 && ActivityStatus != ActivityStatus.Completed) ChangeStatus(ActivityStatus.Completed);

        if (ActivityStatus == ActivityStatus.Completed || ActivityStatus == ActivityStatus.Failed) return "";

        return "Enemy " + (AllEnemiesCount - CurrentRegion.Enemies.Count) + " of " + AllEnemiesCount;
    }
}