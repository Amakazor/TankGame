using SFML.System;
using TankGame.Src.Data;

namespace TankGame.Src.Actors.Pawns.Enemies
{
    internal class LightTank : Enemy
    {
        public LightTank(Vector2f position, Vector2f size, int health = 1) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "enemy1"), health)
        {
        }
    }
}