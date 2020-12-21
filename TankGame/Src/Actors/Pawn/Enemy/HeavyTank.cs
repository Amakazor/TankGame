using SFML.System;
using TankGame.Src.Actors.Pawn.MovementControllers;
using TankGame.Src.Data;

namespace TankGame.Src.Actors.Pawn.Enemy
{
    internal class HeavyTank : Enemy
    {
        public HeavyTank(Vector2f position, Vector2f size, AIMovementController aIMovementController) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "enemy3"))
        {
            MovementController = aIMovementController;
            SetHealth(1);
        }
    }
}