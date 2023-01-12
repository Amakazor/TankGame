using System;
using SFML.Window;
using TankGame.Actors;
using TankGame.Actors.Pawns;
using TankGame.Actors.Pawns.Player;
using TankGame.Actors.Projectiles;
using Action = TankGame.Core.Controls.Action;

namespace TankGame.Events;

public static class MessageBus {
    public delegate void ActionDelegate(Action action);

    public delegate void ContinueDelegate(bool continueGame);

    public delegate void EventArgsDelegate<in T>(T eventArgs) where T : EventArgs;

    public delegate void HealthDelegate(int currentHealth);

    public delegate void NotifyDelegate();

    public delegate void SenderDelegate<in T>(T sender);

    public static NotifyDelegate Quit { get; set; } = delegate { };
    public static SenderDelegate<ITickable> RegisterTickable { get; set; } = delegate { };
    public static SenderDelegate<ITickable> UnregisterTickable { get; set; } = delegate { };

    public static SenderDelegate<IClickable> RegisterClickable { get; set; } = delegate { };
    public static SenderDelegate<IClickable> UnregisterClickable { get; set; } = delegate { };

    public static SenderDelegate<IRenderable> RegisterRenderable { get; set; } = delegate { };
    public static SenderDelegate<IRenderable> UnregisterRenderable { get; set; } = delegate { };

    public static SenderDelegate<IDestructible> RegisterDestructible { get; set; } = delegate { };
    public static SenderDelegate<IDestructible> UnregisterDestructible { get; set; } = delegate { };

    public static SenderDelegate<Projectile> RegisterProjectile { get; set; } = delegate { };
    public static SenderDelegate<Projectile> UnregisterProjectile { get; set; } = delegate { };

    public static EventArgsDelegate<PawnMovedEventArgs> PawnMoved { get; set; } = delegate { };
    public static SenderDelegate<Pawn> PawnDeath { get; set; } = delegate { };
    public static SenderDelegate<Player> PlayerMoved { get; set; } = delegate { };

    public static ActionDelegate Action { get; set; } = delegate { };
    public static EventArgsDelegate<KeyEventArgs> KeyPressed { get; set; } = delegate { };

    public static HealthDelegate PlayerHealthChanged { get; set; } = delegate { };

    public static ContinueDelegate StartGame { get; set; } = delegate { };
    public static NotifyDelegate StopGame { get; set; } = delegate { };

    public static NotifyDelegate CancelInputs { get; set; } = delegate { };
    public static EventArgsDelegate<TextEventArgs> TextInput { get; set; } = delegate { };

    public static NotifyDelegate MenuRefreshKeys { get; set; } = delegate { };
}