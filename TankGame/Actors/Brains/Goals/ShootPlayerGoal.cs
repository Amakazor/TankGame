using System.Linq;
using LanguageExt;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.Pawns;
using TankGame.Actors.Pawns.Player;
using TankGame.Actors.Projectiles;
using TankGame.Core.Gamestate;
using TankGame.Extensions;

namespace TankGame.Actors.Brains.Goals; 

public class ShootPlayerGoal : Goal {
    public ShootPlayerGoal(Brain brain) : base(brain) { }

    public override Option<Thought> NextThought() {
        return GamestateManager.Player.Match<Option<Thought>>(
            player => {
                if (!IsCloseEnough(player)) return None;
                if (!player.Coords.IsInLine(Brain.Owner.Coords)) return None;
                if (!(Brain.Owner as ICoordinated).HasClearLineOfSightTo(player)) return None;
                Direction directionFrom = player.GetDirectionFrom(Brain.Owner.Coords);
                if (directionFrom != Brain.Owner.Direction) return new RotateThought(Brain, 0.2f, directionFrom);
                return new ShootThought(Brain, 1.0f);
            }, None);
    }

    private bool IsCloseEnough(Pawn player) {
        float squareDistanceToTarget = player.Coords.SquareEuclideanDistance(Brain.Owner.Coords);
        if (squareDistanceToTarget    > Brain.Owner.SquareSightDistance) return false;
        return squareDistanceToTarget <= Projectile.SquareFlightDistanceInTiles;
    }
}