using System.Collections.Generic;
using System.Linq;
using SFML.System;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.Fields;
using TankGame.Actors.Pawns;
using TankGame.Core.Gamestate;
using TankGame.Extensions;
using TankGame.Pathfinding;

namespace TankGame.Actors.Brains.Goals; 

public class ChaseTowerGoal : Goal {
    public ChaseTowerGoal(Brain? brain) : base(brain) { }

    private Vector2i? CachedOwnerPosition { get; set; }
    private Stack<Node> Path { get; set; } = new();
    
    public override Thought? NextThought() {
        if (Brain.Owner?.CurrentRegion is null) return null;
        ValidatePath();

        if (Brain.Owner.CurrentRegion.Activity.Coords.SquareEuclideanDistance(Brain.Owner.Coords) > Brain.Owner.SquareSightDistance) return null;
        
        switch (Path.Count) {
            case 0:
                CachedOwnerPosition = Brain.Owner.Coords;
                Path = AStar.FindPath(GamestateManager.Map.GetNodesInRadius(Brain.Owner.Coords, Brain.Owner.SightDistance), Brain.Owner.Coords, Brain.Owner.CurrentRegion.Activity.Coords);
                goto default;
            default:
                Direction newDirection = Path.Peek().GetDirectionFrom((Vector2i)CachedOwnerPosition!);
                if (newDirection != Brain.Owner.Direction) return new RotateThought(Brain, 1, newDirection);
                
                Field? baseField = GamestateManager.Map.GetFieldFromRegion(Brain.Owner.Coords);
                Field? targetField = GamestateManager.Map.GetFieldFromRegion(Path.Peek().Coords);
                if (baseField is null || targetField is null || !targetField.IsTraversible()) return null;
                
                Path.Pop();
                return new MoveThought(Brain, 1, baseField, targetField);
        }
    }
    
    private void ValidatePath() {
        if (Path.Count > 0 && !CheckAndUpdateCachedOwnerPosition()) Path.Clear();
    }

    private bool CheckAndUpdateCachedOwnerPosition() {
        if (CachedOwnerPosition is null || Brain?.Owner is null) return false;

        var ownerCoords = Brain.Owner.Coords;
        if (CachedOwnerPosition == ownerCoords) return true;
        
        CachedOwnerPosition = ownerCoords;
        
        return true;
    }
}