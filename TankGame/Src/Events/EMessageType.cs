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
        PlayerMoved,
        PawnMoved,
        KeyAction,
        KeyPressed
    }
}