using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Extensions;

namespace TankGame.Src.Pathfinding
{
    internal class AStar
    {
        private readonly List<List<Node>> Grid;
        private int GridRows => Grid[0].Count;
        private int GridColumns => Grid.Count;

        public AStar(List<List<Node>> grid)
        {
            Grid = grid;
        }

        public Stack<Node> FindPath(Vector2i Start, Vector2i End)
        {
            Node start = new Node(Start);
            Node end = new Node(End);

            Stack<Node> path = new Stack<Node>();
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();
            List<Node> adjacentNodes;
            Node currentNode = start;

            openList.Add(start);

            while (openList.Count != 0 && !closedList.Exists(x => x.Position == end.Position))
            {
                currentNode = openList[0];
                openList.Remove(currentNode);
                closedList.Add(currentNode);
                adjacentNodes = GetAdjacentNodes(currentNode);

                foreach (Node adjacentNode in adjacentNodes)
                {
                    if (!closedList.Contains(adjacentNode) && adjacentNode.Walkable)
                    {
                        if (!openList.Contains(adjacentNode))
                        {
                            adjacentNode.Parent = currentNode;
                            adjacentNode.DistanceToTarget = adjacentNode.Position.ManhattanDistance(end.Position);
                            adjacentNode.Cost = adjacentNode.Weight + adjacentNode.Parent.Cost;
                            openList.Add(adjacentNode);
                            openList = openList.OrderBy(node => node.F).ToList();
                        }
                    }
                }
            }

            if (!closedList.Exists(x => x.Position == end.Position)) return null;

            do
            {
                path.Push(currentNode);
                currentNode = currentNode.Parent;
            } while (currentNode != start && currentNode != null);
            return path;
        }

        private List<Node> GetAdjacentNodes(Node node)
        {
            List<Node> adjacentNodes = new List<Node>();

            int row = node.Position.Y;
            int col = node.Position.X;

            if (row + 1 < GridRows)    adjacentNodes.Add(Grid[col][row + 1]);
            if (row - 1 >= 0)          adjacentNodes.Add(Grid[col][row - 1]);
            if (col - 1 >= 0)          adjacentNodes.Add(Grid[col - 1][row]);
            if (col + 1 < GridColumns) adjacentNodes.Add(Grid[col + 1][row]);

            return adjacentNodes;
        }
    }
}