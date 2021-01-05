namespace TankGame.Src.Events
{
    internal enum MessageType
    {
        Quit,
        RegisterTickable,
        UnregisterTickable,
        RegisterRenderable,
        UnregisterRenderable,
        RegisterClickable,
        UnregisterClickable,
        RegisterDestructible,
        UnregisterDestructible,
        RegisterProjectile,
        UnregisterProjectile,
        PlayerMoved,
        PawnMoved,
        KeyAction,
        KeyPressed,
        PlayerHealthChanged,
        UpdateActivityDisplay
    }
}