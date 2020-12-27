using SFML.System;

namespace TankGame.Src.Pathfinding
{
    internal class Node
    {
        public Node Parent { get; set; }
        public Vector2i Position { get; }
        public float DistanceToTarget { get; set; }
        public float Cost { get; set; }
        public float Weight { get; }
        public bool Walkable { get; }
        public float F => (DistanceToTarget != -1 && Cost != -1) ? (DistanceToTarget + Cost) : -1;

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