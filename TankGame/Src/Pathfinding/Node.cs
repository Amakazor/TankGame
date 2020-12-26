using SFML.System;

namespace TankGame.Src.Pathfinding
{
    internal class Node
    {
        public Node Parent;
        public Vector2i Position;
        public float DistanceToTarget;
        public float Cost;
        public float Weight;
        public bool Walkable;
        public float F
        {
            get
            {
                return (DistanceToTarget != -1 && Cost != -1) ? (DistanceToTarget + Cost) : -1;
            }
        }

        public Node(Vector2i position, bool walkable = true, float weight = 1)
        {
            Parent = null;
            Position = position;
            DistanceToTarget = -1;
            Cost = 1;
            Weight = weight;
            Walkable = walkable;
        }
    }
}