using SFML.System;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using TankGame.Src.Actors;
using TankGame.Src.Actors.GameObjects;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Actors.Projectiles;
using TankGame.Src.Events;
using TankGame.Src.Extensions;

namespace TankGame.Src.Data
{
    internal class CollisionManager : IDisposable
    {
        private HashSet<IDestructible> Destructibles { get; }
        private HashSet<Projectile> Projectiles { get; }

        private HashSet<IDestructible> DestructiblesToAdd { get; }
        private HashSet<Projectile> ProjectilesToAdd { get; }

        private HashSet<IDestructible> DestructiblesToDelete { get; }
        private HashSet<Projectile> ProjectilesToDelete { get; }

        public CollisionManager()
        {
            Destructibles = new HashSet<IDestructible>();
            DestructiblesToAdd = new HashSet<IDestructible>();
            DestructiblesToDelete = new HashSet<IDestructible>();
            
            Projectiles = new HashSet<Projectile>();
            ProjectilesToAdd = new HashSet<Projectile>();
            ProjectilesToDelete = new HashSet<Projectile>();

            MessageBus messageBus = MessageBus.Instance;

            messageBus.Register(MessageType.RegisterDestructible, OnRegisterDestructible);
            messageBus.Register(MessageType.UnregisterDestructible, OnUnregisterDestructible);
            messageBus.Register(MessageType.RegisterProjectile, OnRegisterProjectile);
            messageBus.Register(MessageType.UnregisterProjectile, OnUnregisterProjectile);
        }

        public void Tick()
        {
            Projectiles.AddDeleteRange(ProjectilesToAdd, ProjectilesToDelete);
            Destructibles.AddDeleteRange(DestructiblesToAdd, DestructiblesToDelete);

            foreach (Projectile projectile in Projectiles)
            {
                foreach (IDestructible destructible in Destructibles)
                {
                    if (destructible.IsDestructible && destructible.IsAlive && CheckCollision(projectile, destructible.Actor))
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
            if (sender is IDestructible destructible) DestructiblesToAdd.Add(destructible);
        }

        private void OnUnregisterDestructible(object sender, EventArgs eventArgs)
        {
            if (sender is IDestructible destructible && Destructibles.Contains(destructible)) DestructiblesToDelete.Add(destructible);
        }

        private void OnRegisterProjectile(object sender, EventArgs eventArgs)
        {
            if (sender is Projectile projectile) ProjectilesToAdd.Add(projectile);
        }

        private void OnUnregisterProjectile(object sender, EventArgs eventArgs)
        {
            if (sender is Projectile projectile && Projectiles.Contains(projectile)) ProjectilesToDelete.Add(projectile);
        }

        public void Dispose()
        {
            MessageBus messageBus = MessageBus.Instance;

            Destructibles.Clear();
            DestructiblesToAdd.Clear();
            DestructiblesToDelete.Clear();

            Projectiles.Clear();
            ProjectilesToAdd.Clear();
            ProjectilesToDelete.Clear();

            messageBus.Unregister(MessageType.RegisterDestructible, OnRegisterDestructible);
            messageBus.Unregister(MessageType.UnregisterDestructible, OnUnregisterDestructible);
            messageBus.Unregister(MessageType.RegisterProjectile, OnRegisterProjectile);
            messageBus.Unregister(MessageType.UnregisterProjectile, OnUnregisterProjectile);
        }
    }
}
