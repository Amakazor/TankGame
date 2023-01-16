using System;
using SFML.Window;

namespace TankGame.Core.Controls;

public class KeyAction : IEquatable<KeyAction> {
    public KeyAction(Keyboard.Key key, InputAction inputAction) {
        Key = key;
        InputAction = inputAction;
    }

    public InputAction InputAction { get; }
    public Keyboard.Key Key { get; }

    public bool Equals(KeyAction? other)
        => InputAction.Equals(other?.InputAction);

    public override bool Equals(object? obj)
        => obj is KeyAction other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Key, InputAction);

    public static bool operator ==(KeyAction left, KeyAction right)
        => left.Equals(right);

    public static bool operator !=(KeyAction left, KeyAction right)
        => !left.Equals(right);
}