using System;
using SFML.System;
using TankGame.Actors.Pawns;

namespace TankGame.Pathfinding;

public sealed class Node : IEquatable<Node> {
    public Node(Vector2i coords, bool walkable = true, float weight = 1) {
        Parent = null;
        Coords = coords;
        Cost = 1;
        Weight = weight;
        Walkable = walkable;
    }

    public Node? Parent { get; set; }
    public Vector2i Coords { get; }
    public float DistanceToTarget { get; set; }
    public float Cost { get; set; }
    public float Weight { get; }
    public bool Walkable { get; }
    public float HeuristicF => DistanceToTarget != 0F ? DistanceToTarget + Cost : float.MaxValue;
    
    public Direction GetDirectionFrom(Vector2i position) {
        if (position.X > Coords.X) return Direction.Left;
        if (position.X < Coords.X) return Direction.Right;
        if (position.Y > Coords.Y) return Direction.Up;
        return Direction.Down;
    }

    public bool Equals(Node other)
        => Coords.Equals(other.Coords);

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Node)obj);
    }

    public override int GetHashCode()
        => Coords.GetHashCode();

    public static bool operator ==(Node? left, Node? right)
        => Equals(left, right);
    public static bool operator !=(Node? left, Node? right)
        => !Equals(left, right);

    public override string ToString() {
            return $"Node: {Coords} - {Cost}";
    }
}