using SFML.System;
using TankGame.Src.Actors.Pawn.MovementControllers;
using TankGame.Src.Data;

namespace TankGame.Src.Actors.Pawn.Enemies
{
    internal class HeavyTank : Enemy
    {
        public HeavyTank(Vector2f position, Vector2f size) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "enemy3"))
        {
            SetHealth(3);
        }
    }
}