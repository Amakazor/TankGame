using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;
using TankGame.Actors;
using TankGame.Actors.GameObjects;
using TankGame.Actors.Pawns;
using TankGame.Actors.Pawns.Enemies;
using TankGame.Actors.Pawns.Players;
using TankGame.Actors.Projectiles;
using TankGame.Events;

namespace TankGame.Core.Collisions;

public class CollisionHandler : IDisposable {
    public CollisionHandler() {
        Destructibles = new();
        Projectiles = new();

        MessageBus.RegisterDestructible += OnRegisterDestructible;
        MessageBus.UnregisterDestructible += OnUnregisterDestructible;
        MessageBus.RegisterProjectile += OnRegisterProjectile;
        MessageBus.UnregisterProjectile += OnUnregisterProjectile;
    }

    private HashSet<IDestructible> Destructibles { get; }
    private HashSet<Projectile> Projectiles { get; }

    public void Dispose() {
        Clear();

        MessageBus.RegisterDestructible -= OnRegisterDestructible;
        MessageBus.UnregisterDestructible -= OnUnregisterDestructible;
        MessageBus.RegisterProjectile -= OnRegisterProjectile;
        MessageBus.UnregisterProjectile -= OnUnregisterProjectile;
    }

    public void Tick() {
        foreach (Projectile projectile in Projectiles.ToSeq())
        foreach (IDestructible destructible in Destructibles.Filter(destructible => CheckCollision(projectile, destructible.Actor)).ToSeq())
            if (destructible.DestructabilityType != DestructabilityType.Indestructible)
                switch (destructible.Actor) {
                    case Enemy _ when projectile.Owner is Player:
                    case Player _ when projectile.Owner is Enemy:
                    case GameObject _:
                        destructible.Hit();
                        projectile.Dispose();
                        break;
                }
            else if (destructible.StopsProjectile) projectile.Dispose();
    }

    private static bool CheckCollision(Projectile projectile, Actor actor2)
        => CollidesAabb(projectile.CollisionPosition, projectile.CollistionSize, actor2 is Pawn pawn ? pawn.Position : actor2.Position, actor2.Size);

    private static bool CollidesAabb(Vector2f position1, Vector2f size1, Vector2f position2, Vector2f size2)
        => position1.X + size1.X >= position2.X && position2.X + size2.X >= position1.X && position1.Y + size1.Y >= position2.Y && position2.Y + size2.Y >= position1.Y;

    private void OnRegisterDestructible(IDestructible sender)
        => Destructibles.Add(sender);

    private void OnUnregisterDestructible(IDestructible sender)
        => Destructibles.Remove(sender);

    private void OnRegisterProjectile(Projectile sender)
        => Projectiles.Add(sender);

    private void OnUnregisterProjectile(Projectile sender)
        => Projectiles.Remove(sender);

    public void Clear() {
        Projectiles.ToList().ForEach(projectile => projectile.Dispose());

        Destructibles.Clear();
        Projectiles.Clear();
    }
}