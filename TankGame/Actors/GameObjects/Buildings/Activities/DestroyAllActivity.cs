using SFML.System;
using TankGame.Actors.GameObjects.Buildings.Utility;
using TankGame.Core.Map;

namespace TankGame.Actors.GameObjects.Buildings.Activities;

public class DestroyAllActivity : Activity {
    public new class Dto : Activity.Dto { }
    public DestroyAllActivity(Vector2i coords, string name, int health, int pointsAdded, ActivityStatus activityStatus, int enemiesCount) : base(coords, name, health, pointsAdded, activityStatus, enemiesCount) { }

    public DestroyAllActivity(Dto dto, Region region) : base(dto, region) { }

    protected override string CalculateProgress() {
        // if (AllEnemiesCount == 0 && CurrentRegion.Enemies.Count > 0) AllEnemiesCount = CurrentRegion.Enemies.Count;
        //
        // if (CurrentRegion.Enemies.Count == 0 && ActivityStatus != ActivityStatus.Completed) ChangeStatus(ActivityStatus.Completed);
        //
        // if (ActivityStatus == ActivityStatus.Completed || ActivityStatus == ActivityStatus.Failed) return "";
        //
        // return "Enemy " + (AllEnemiesCount - CurrentRegion.Enemies.Count) + " of " + AllEnemiesCount;
        return "";
    }

    public override DestructabilityType DestructabilityType => DestructabilityType.Indestructible;
}