using SFML.System;
using TankGame.Src.Actors.Pawn.MovementControllers;
using TankGame.Src.Data;

namespace TankGame.Src.Actors.Pawn.Enemies
{
    internal class LightTank : Enemy
    {
        public LightTank(Vector2f position, Vector2f size) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "enemy1"))
        {
            SetHealth(1);
        }
    }
}