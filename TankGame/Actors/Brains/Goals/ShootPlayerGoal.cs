using System.Linq;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.Projectiles;
using TankGame.Core.Gamestate;
using TankGame.Extensions;

namespace TankGame.Actors.Brains.Goals; 

public class ShootPlayerGoal : Goal {
    public ShootPlayerGoal(Brain brain) : base(brain) { }

    public override Thought? NextThought() {
        if (GamestateManager.Player is null) return null;
        float squareDistanceToTarget = GamestateManager.Player.Coords.SquareEuclideanDistance(Brain.Owner.Coords);
        if (squareDistanceToTarget > Brain.Owner.SquareSightDistance) return null;
        if (squareDistanceToTarget > Projectile.SquareFlightDistanceInTiles) return null;
        if (!GamestateManager.Player.Coords.IsInLine(Brain.Owner.Coords)) return null;
        if (GamestateManager.Player.Coords.GetAllVectorsBetween(Brain.Owner.Coords)
                            .Select(coords => GamestateManager.Map.GetFieldFromRegion(coords))
                            .Any(field => field is null || !field.CanBeShootThrough(true))) return null;
        if (GamestateManager.Player.GetDirectionFrom(Brain.Owner.Coords) != Brain.Owner.Direction) 
            return new RotateThought(Brain, 0.2f, GamestateManager.Player.GetDirectionFrom(Brain.Owner.Coords));
        return new ShootThought(Brain, 1.0f);
    }
}