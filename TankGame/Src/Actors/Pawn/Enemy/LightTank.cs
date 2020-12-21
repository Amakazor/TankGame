﻿using SFML.System;
using TankGame.Src.Actors.Pawn.MovementControllers;
using TankGame.Src.Data;

namespace TankGame.Src.Actors.Pawn.Enemy
{
    internal class LightTank : Enemy
    {
        public LightTank(Vector2f position, Vector2f size, AIMovementController aIMovementController) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "enemy1"))
        {
            MovementController = aIMovementController;
            SetHealth(1);
        }
    }
}