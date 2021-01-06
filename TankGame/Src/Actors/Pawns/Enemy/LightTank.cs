using SFML.System;
using TankGame.Src.Data;

namespace TankGame.Src.Actors.Pawns.Enemies
{
    internal class LightTank : Enemy
    {
        public LightTank(Vector2f position, Vector2f size, int health = 1) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "enemy1"), health, 100, "light")
        {
        }

        protected override void UpdatePosition(Vector2i lastCoords, Vector2i newCoords)
        {
            SoundManager.Instance.PlaySound("move", "light", Position / 64);
            base.UpdatePosition(lastCoords, newCoords);
        }
    }
}