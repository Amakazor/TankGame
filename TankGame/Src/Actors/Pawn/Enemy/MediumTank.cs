using SFML.System;
using TankGame.Src.Actors.Pawn.MovementControllers;
using TankGame.Src.Data;

namespace TankGame.Src.Actors.Pawn.Enemies
{
    internal class MediumTank : Enemy
    {
        public MediumTank(Vector2f position, Vector2f size) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "enemy2"))
        {
            SetHealth(2);
        }
    }
}