﻿using SFML.Graphics;
using SFML.System;
using TankGame.Src.Data;
using TankGame.Src.Data.Map;

namespace TankGame.Src.Actors.Pawns.Enemies
{
    abstract internal class Enemy : Pawn
    {
        uint ScoreAdded { get; }

        public Enemy(Vector2f position, Vector2f size, Texture texture, int health, uint scoreAdded) : base(position, size, texture, health)
        {
            ScoreAdded = scoreAdded;
        }

        public override void OnDestroy()
        {
            GamestateManager.Instance.AddPoints(ScoreAdded, RealPosition + new Vector2f((Size.X / 2) - 75, ((Size.Y / 10) - 10)));
            GamestateManager.Instance.Map.GetRegionFromFieldCoords(Coords).OnEnemyDestruction(this);
            base.OnDestroy();
        }
    }
}
