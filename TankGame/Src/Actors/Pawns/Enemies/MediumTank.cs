using SFML.System;
using TankGame.Src.Data.Textures;

namespace TankGame.Src.Actors.Pawns.Enemies
{
    internal class MediumTank : Enemy
    {
        public MediumTank(Vector2f position, Vector2f size, int health = 2) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "enemy2"), health, 200, "medium", "medium")
        {
        }
    }
}