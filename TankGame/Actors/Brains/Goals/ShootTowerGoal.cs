using LanguageExt;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.Pawns;
using TankGame.Actors.Projectiles;
using TankGame.Extensions;

namespace TankGame.Actors.Brains.Goals; 

public class ShootTowerGoal : Goal {
    public new class Dto : Goal.Dto { }
    
    public ShootTowerGoal(Brain brain) : base(brain) { }
    
    public ShootTowerGoal(Brain brain, Dto dto) : base(brain, dto) { }
    
    public override Option<Thought> NextThought() {
        return Brain.Owner.CurrentRegion.Match<Option<Thought>>(
            region => {
                if (!IsCloseEnough(region.Activity)) return None;
                if (!region.Activity.Coords.IsInLine(Brain.Owner.Coords)) return None;
                if (!(Brain.Owner as ICoordinated).HasClearLineOfSightTo(region.Activity)) return None;
                Direction directionFrom = region.Activity.GetDirectionFrom(Brain.Owner.Coords);
                if (directionFrom != Brain.Owner.Direction) return new RotateThought(Brain, 0.2f, directionFrom);
                return new ShootThought(Brain, 1.0f);
            }, None);
        
    }
    
    private bool IsCloseEnough(ICoordinated gameObject) {
        float squareDistanceToTarget = gameObject.Coords.SquareEuclideanDistance(Brain.Owner.Coords);
        if (squareDistanceToTarget    > Brain.Owner.SquareSightDistance) return false;
        return squareDistanceToTarget <= Projectile.SquareFlightDistanceInTiles;
    }
}