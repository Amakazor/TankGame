using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Actors;
using TankGame.Src.Actors.GameObjects;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Actors.Projectiles;
using TankGame.Src.Events;

namespace TankGame.Src.Data.Collisions
{
    internal class CollisionHandler : IDisposable
    {
        private HashSet<IDestructible> Destructibles { get; }
        private HashSet<Projectile> Projectiles { get; }

        public CollisionHandler()
        {
            Destructibles = new HashSet<IDestructible>();
            
            Projectiles = new HashSet<Projectile>();

            MessageBus messageBus = MessageBus.Instance;

            messageBus.Register(MessageType.RegisterDestructible, OnRegisterDestructible);
            messageBus.Register(MessageType.UnregisterDestructible, OnUnregisterDestructible);
            messageBus.Register(MessageType.RegisterProjectile, OnRegisterProjectile);
            messageBus.Register(MessageType.UnregisterProjectile, OnUnregisterProjectile);
        }

        public void Tick()
        {
            foreach (Projectile projectile in Projectiles.ToList())
            {
                foreach (IDestructible destructible in Destructibles.ToList())
                {
                    if (CheckCollision(projectile, destructible.Actor))
                    {
                        if (destructible.IsDestructible && destructible.IsAlive)
                        {
                            switch (destructible.Actor)
                            {
                                case Enemy _:
                                    if (projectile.Owner is Player)
                                    {
                                        destructible.OnHit();
                                        projectile.Dispose();
                                    }
                                    break;
                                case Player _:
                                    if (projectile.Owner is Enemy)
                                    {
                                        destructible.OnHit();
                                        projectile.Dispose();
                                    }
                                    break;
                                case GameObject _:
                                    destructible.OnHit();
                                    projectile.Dispose();
                                    break;
                            }
                        }
                        else if (destructible.StopsProjectile) projectile.Dispose();
                    }
                }
            }
        }

        private bool CheckCollision(Projectile projectile, Actor actor2) => CollidesAABB(projectile.CollisionPosition, projectile.CollistionSize, (actor2 is Pawn pawn) ? pawn.RealPosition : actor2.Position, actor2.Size);

        private bool CollidesAABB(Vector2f position1, Vector2f size1, Vector2f position2, Vector2f size2)
        {
            return position1.X + size1.X >= position2.X
                && position2.X + size2.X >= position1.X
                && position1.Y + size1.Y >= position2.Y
                && position2.Y + size2.Y >= position1.Y;
        }

        private void OnRegisterDestructible(object sender, EventArgs eventArgs)
        {
            if (sender is IDestructible destructible) Destructibles.Add(destructible);
        }

        private void OnUnregisterDestructible(object sender, EventArgs eventArgs)
        {
            if (sender is IDestructible destructible) Destructibles.Remove(destructible);
        }

        private void OnRegisterProjectile(object sender, EventArgs eventArgs)
        {
            if (sender is Projectile projectile) Projectiles.Add(projectile);
        }

        private void OnUnregisterProjectile(object sender, EventArgs eventArgs)
        {
            if (sender is Projectile projectile) Projectiles.Remove(projectile);
        }

        public void Clear()
        {
            Projectiles.ToList().ForEach(projectile => projectile?.Dispose());

            Destructibles.Clear();
            Projectiles.Clear();
        }

        public void Dispose()
        {
            MessageBus messageBus = MessageBus.Instance;

            Clear();

            messageBus.Unregister(MessageType.RegisterDestructible, OnRegisterDestructible);
            messageBus.Unregister(MessageType.UnregisterDestructible, OnUnregisterDestructible);
            messageBus.Unregister(MessageType.RegisterProjectile, OnRegisterProjectile);
            messageBus.Unregister(MessageType.UnregisterProjectile, OnUnregisterProjectile);
        }
    }
}
