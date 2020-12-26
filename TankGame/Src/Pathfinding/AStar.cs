using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TankGame.Src.Pathfinding
{
    internal class AStar
    {
        List<List<Node>> Grid;
        int GridRows { get { return Grid[0].Count; } }
        int GridColumns { get { return Grid.Count; } }

        public AStar(List<List<Node>> grid)
        {
            Grid = grid;
        }

        public Stack<Node> FindPath(Vector2i Start, Vector2i End)
        {
            Node start = new Node(Start);
            Node end = new Node(End);

            Stack<Node> Path = new Stack<Node>();
            List<Node> OpenList = new List<Node>();
            List<Node> ClosedList = new List<Node>();
            List<Node> adjacencies;
            Node current = start;

            OpenList.Add(start);

            while (OpenList.Count != 0 && !ClosedList.Exists(x => x.Position == end.Position))
            {
                current = OpenList[0];
                OpenList.Remove(current);
                ClosedList.Add(current);
                adjacencies = GetAdjacentNodes(current);


                foreach (Node n in adjacencies)
                {
                    if (!ClosedList.Contains(n) && n.Walkable)
                    {
                        if (!OpenList.Contains(n))
                        {
                            n.Parent = current;
                            n.DistanceToTarget = Math.Abs(n.Position.X - end.Position.X) + Math.Abs(n.Position.Y - end.Position.Y);
                            n.Cost = n.Weight + n.Parent.Cost;
                            OpenList.Add(n);
                            OpenList = OpenList.OrderBy(node => node.F).ToList<Node>();
                        }
                    }
                }
            }

            if (!ClosedList.Exists(x => x.Position == end.Position)) return null;

            Node temp = ClosedList[ClosedList.IndexOf(current)];
            if (temp == null) return null;
            do
            {
                Path.Push(temp);
                temp = temp.Parent;
            } while (temp != start && temp != null);
            return Path;
        }

        public Node GetFirstNodeInPath(Vector2i Start, Vector2i End)
        {
            return FindPath(Start, End)?.Peek();
        }

        private List<Node> GetAdjacentNodes(Node n)
        {
            List<Node> temp = new List<Node>();

            int row = n.Position.Y;
            int col = n.Position.X;

            if (row + 1 < GridRows)
            {
                temp.Add(Grid[col][row + 1]);
            }
            if (row - 1 >= 0)
            {
                temp.Add(Grid[col][row - 1]);
            }
            if (col - 1 >= 0)
            {
                temp.Add(Grid[col - 1][row]);
            }
            if (col + 1 < GridColumns)
            {
                temp.Add(Grid[col + 1][row]);
            }

            return temp;
        }
    }
}
