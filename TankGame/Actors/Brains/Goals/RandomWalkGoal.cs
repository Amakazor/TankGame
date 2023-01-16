using System;
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

public class RandomWalkGoal : Goal {
    private Random Random { get; }
    private Vector2i? CachedOwnerPosition { get; set; }
    private Stack<Node> Path { get; set; } = new();

    public RandomWalkGoal(Brain brain) : base(brain) {
        Random = new();
    }

    public override Thought? NextThought() {
        if (Brain.Owner?.CurrentRegion is null) return null;
        ValidatePath();

        switch (Path.Count) {
            case 0:
                var nodes = Brain.Owner.CurrentRegion.GetNodesInRegion();
                CachedOwnerPosition = Brain.Owner.Coords;

                bool createdPath = nodes.OrderRandomly().Where(node => node.Walkable).FirstOrDefault(
                     node => {
                         Path = AStar.FindPath(nodes, Brain.Owner.Coords, node.Coords);
                         return Path.Count > 0;
                     }, null
                 ) is not null;
                if (!createdPath) return null;

                goto default;
            default:
                Direction newDirection = Path.Peek().GetDirectionFrom((Vector2i)CachedOwnerPosition!);
                if (newDirection != Brain.Owner.Direction) {
                    return new RotateThought(Brain, 1, newDirection);
                }
                
                Field? baseField = GamestateManager.Map.GetFieldFromRegion(Brain.Owner.Coords);
                Field? targetField = GamestateManager.Map.GetFieldFromRegion(Path.Peek().Coords);
                
                if (baseField is not null && targetField is not null && targetField.IsTraversible()) {
                    Path.Pop();
                    return new MoveThought(Brain, 1, baseField, targetField);
                }
                
                Path.Clear();
                return null;
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