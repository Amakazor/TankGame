using System;
using TankGame.Core.Map;

namespace TankGame.Actors.GameObjects.Buildings.Activities; 

public static class ActivityFactory {
    public static Activity Create(Activity.Dto activityDto, Region region)
        => activityDto switch {
            ProtectActivity.Dto dto     => new ProtectActivity(dto, region),
            DestroyAllActivity.Dto dto  => new DestroyAllActivity(dto, region),
            WaveProtectActivity.Dto dto => new WaveProtectActivity(dto, region),
            WaveActivity.Dto dto        => new WaveActivity(dto, region),
            _                           => throw new ArgumentOutOfRangeException(nameof(activityDto), activityDto, null)
        };
}