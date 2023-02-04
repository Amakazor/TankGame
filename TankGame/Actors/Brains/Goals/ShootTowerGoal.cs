using System.Linq;
using LanguageExt;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.GameObjects;
using TankGame.Actors.Pawns;
using TankGame.Actors.Projectiles;
using TankGame.Core.Gamestate;
using TankGame.Extensions;

namespace TankGame.Actors.Brains.Goals; 

public class ShootTowerGoal : Goal {
    public ShootTowerGoal(Brain brain) : base(brain) { }
    
    public override Option<Thought> NextThought() {
        return Brain.Owner.CurrentRegion.Match<Option<Thought>>(
            region => {
                IsCloseEnough(region.Activity);
                if (!region.Activity.Coords.IsInLine(Brain.Owner.Coords)) return None;
                if (!(Brain.Owner as ICoordinated).HasClearLineOfSightTo(region.Activity)) return None;
                Direction directionFrom = region.Activity.GetDirectionFrom(Brain.Owner.Coords);
                if (directionFrom != Brain.Owner.Direction) return new RotateThought(Brain, 0.2f, directionFrom);
                return new ShootThought(Brain, 1.0f);
            }, None);
        
    }
    
    private bool IsCloseEnough(GameObject gameObject) {
        float squareDistanceToTarget = gameObject.Coords.SquareEuclideanDistance(Brain.Owner.Coords);
        if (squareDistanceToTarget    > Brain.Owner.SquareSightDistance) return false;
        return squareDistanceToTarget <= Projectile.SquareFlightDistanceInTiles;
    }
}