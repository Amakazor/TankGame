using SFML.System;
using TankGame.Actors.GameObjects.Buildings.Utility;
using TankGame.Core.Map;

namespace TankGame.Actors.GameObjects.Buildings.Activities;

public class ProtectActivity : DestroyAllActivity {
    public new class Dto : DestroyAllActivity.Dto { }
    public ProtectActivity(Vector2i coords, string name, int health, int pointsAdded, ActivityStatus activityStatus, int enemiesCount) : base(coords, name, health, pointsAdded, activityStatus, enemiesCount) { }
    public ProtectActivity(Dto dto, Region region) : base(dto, region) { }
    
    public override DestructabilityType DestructabilityType => DestructabilityType.Destructible;
}