using SFML.Graphics;
using SFML.System;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Data;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Projectiles
{
    internal class EnemyProjectile : Projectile
    {
        public EnemyProjectile(Vector2f position, Direction direction) : base(position, direction)
        {
            ProjectileComponent = new SpriteComponent(Position, Size, this, TextureManager.Instance.GetTexture(TextureType.Projectile, "pocisk1"), new Color(255, 255, 255, 255));
            ProjectileComponent.SetDirection(Direction);
        }
    }
}