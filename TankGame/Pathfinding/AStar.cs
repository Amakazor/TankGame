using System.Collections.Generic;
using System.Linq;
using SFML.System;
using TankGame.Extensions;

namespace TankGame.Pathfinding;

public static class AStar {

    public static Stack<Node> FindPath(ISet<Node> grid, Vector2i startCoords, Vector2i endCoords) {
        var start = grid.FirstOrDefault(node => node.Coords == startCoords);
        var end = grid.FirstOrDefault(node => node.Coords == endCoords);
        if (start is null || end is null) return new();

        List<Node> openList = new();
        HashSet<Node> closedList = new();

        openList.Add(start);

        while (openList.Count != 0) {
            Node currentNode = openList.MinBy(node => node.HeuristicF)!;
            
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            if (currentNode == end) break;

            foreach (Node adjacentNode in GetAdjacentNodes(currentNode, grid).Where(node => !closedList.Contains(node) && node.Walkable && !openList.Contains(node))) {
                adjacentNode.Parent = currentNode;
                adjacentNode.DistanceToTarget = adjacentNode.Coords.ManhattanDistance(end.Coords);
                adjacentNode.Cost = adjacentNode.Weight + adjacentNode.Parent.Cost;
                openList.Add(adjacentNode);
            }
        }
        
        var toReturn = closedList.Any(x => x == end) ? RetracePath(start, end) : new();

        return toReturn;
    }

    public static Stack<Node> RetracePath(Node start, Node end) {
        Stack<Node> path = new();
        
        Node? currentNode = end;
        do {
            path.Push(currentNode);
            currentNode = currentNode.Parent;
        } while (currentNode != start && currentNode != null);

        return path;
    }

    public static Vector2i FindNextStep(HashSet<Node> grid, Vector2i start, Vector2i end) {
        var path = FindPath(grid, start, end);
        return path.Count == 0 ? start : path.Pop().Coords;
    }
    
    private static IEnumerable<Vector2i> GetAdjacentPositions(Vector2i position) {
        yield return new(position.X - 1, position.Y);
        yield return new(position.X + 1, position.Y);
        yield return new(position.X, position.Y - 1);
        yield return new(position.X, position.Y + 1);
    }

    private static IEnumerable<Node> GetAdjacentNodes(Node node, ISet<Node> grid) {
        return GetAdjacentPositions(node.Coords)
              .Select(position => grid.FirstOrDefault(x => x!.Coords == position, null))
              .Where(x => x is not null)
              .OfType<Node>();
    }
}