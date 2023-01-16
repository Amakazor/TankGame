using System.Linq;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.Projectiles;
using TankGame.Core.Gamestate;
using TankGame.Extensions;

namespace TankGame.Actors.Brains.Goals; 

public class ShootTowerGoal : Goal {
    public ShootTowerGoal(Brain brain) : base(brain) { }
    
    public override Thought? NextThought() {
        if (Brain.Owner?.CurrentRegion is null) return null;
        float squareDistanceToTarget = Brain.Owner.CurrentRegion.Activity.Coords.SquareEuclideanDistance(Brain.Owner.Coords);
        if (squareDistanceToTarget > Brain.Owner.SquareSightDistance) return null;
        if (squareDistanceToTarget > Projectile.SquareFlightDistanceInTiles) return null;
        if (!Brain.Owner.CurrentRegion.Activity.Coords.IsInLine(Brain.Owner.Coords)) return null;
        if (Brain.Owner.CurrentRegion.Activity.Coords.GetAllVectorsBetween(Brain.Owner.Coords)
                 .Select(coords => GamestateManager.Map.GetFieldFromRegion(coords))
                 .Any(field => field is null || !field.CanBeShootThrough(true))) return null;
        return new ShootThought(Brain, 1.0f);
    }
}