using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Data;

namespace TankGame.Src.Actors.Pawn.Enemy
{
    class LightTank : Enemy
    {
        public LightTank(Vector2f position, Vector2f size) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "enemy1"))
        {
        }
    }
}
