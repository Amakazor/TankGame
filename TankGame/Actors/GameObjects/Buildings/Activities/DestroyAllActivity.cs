using SFML.System;
using TankGame.Actors.GameObjects.Buildings.Utility;
using TankGame.Core.Map;

namespace TankGame.Actors.GameObjects.Buildings.Activities;

public class DestroyAllActivity : Activity {
    public new class Dto : Activity.Dto { }
    public DestroyAllActivity(Vector2i coords, string name, int health, int pointsAdded, ActivityStatus activityStatus, int enemiesCount) : base(coords, name, health, pointsAdded, activityStatus, enemiesCount) { }

    public DestroyAllActivity(Dto dto, Region region) : base(dto, region) { }

    protected override string CalculateProgress() {
        return "";
    }

    public override DestructabilityType DestructabilityType => DestructabilityType.Indestructible;
}