using SFML.Graphics;
using SFML.System;

namespace TankGame.Src.Actors.Pawns.Enemies
{
    abstract internal class Enemy : Pawn
    {
        public Enemy(Vector2f position, Vector2f size, Texture texture, int health) : base(position, size, texture, health)
        {
        }
    }
}
