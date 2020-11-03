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
        KeyAction,
        KeyPressed
    }
}