using System;
using SFML.Window;

namespace TankGame.Core.Controls;

public class KeyAction : IEquatable<KeyAction> {
    public KeyAction(Keyboard.Key key, Action action) {
        Key = key;
        Action = action;
    }

    public Action Action { get; }
    public Keyboard.Key Key { get; }

    public bool Equals(KeyAction? other)
        => Action.Equals(other?.Action);

    public override bool Equals(object? obj)
        => obj is KeyAction other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Key, Action);

    public static bool operator ==(KeyAction left, KeyAction right)
        => left.Equals(right);

    public static bool operator !=(KeyAction left, KeyAction right)
        => !left.Equals(right);
}