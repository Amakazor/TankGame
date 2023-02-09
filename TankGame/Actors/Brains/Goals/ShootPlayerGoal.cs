using LanguageExt;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.Pawns;
using TankGame.Actors.Projectiles;
using TankGame.Core.Gamestates;
using TankGame.Extensions;

namespace TankGame.Actors.Brains.Goals; 

public class ShootPlayerGoal : Goal {
    public new class Dto : Goal.Dto { }
    
    public ShootPlayerGoal(Brain brain) : base(brain) { }
    
    public ShootPlayerGoal(Brain brain, Dto dto) : base(brain, dto) { }

    public override Option<Thought> NextThought() {
        return Gamestate.Player.Match<Option<Thought>>(
            player => {
                if (!IsCloseEnough(player)) return None;
                if (!player.Coords.IsInLine(Brain.Owner.Coords)) return None;
                if (!(Brain.Owner as ICoordinated).HasClearLineOfSightTo(player)) return None;
                Direction directionFrom = player.GetDirectionFrom(Brain.Owner.Coords);
                if (directionFrom != Brain.Owner.Direction) return new RotateThought(Brain, 0.2f, directionFrom);
                return new ShootThought(Brain, 1.0f);
            }, None);
    }

    private bool IsCloseEnough(ICoordinated player) {
        float squareDistanceToTarget = player.Coords.SquareEuclideanDistance(Brain.Owner.Coords);
        if (squareDistanceToTarget    > Brain.Owner.SquareSightDistance) return false;
        return squareDistanceToTarget <= Projectile.SquareFlightDistanceInTiles;
    }
}