﻿using SFML.System;
using System;
using TankGame.Src.Actors.Pawns.MovementControllers;
using TankGame.Src.Data;
using TankGame.Src.Events;

namespace TankGame.Src.Actors.Pawns.Player
{
    internal class Player : Pawn
    {
        public Player(Vector2f position, Vector2f size, int health = 3) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "player1"), health)
        {
            MovementController = new PlayerMovementController(0.5F, this);
            MessageBus.Instance.PostEvent(MessageType.PlayerMoved, this, new EventArgs());
        }

        protected override void UpdatePosition(Vector2i lastCoords, Vector2i newCoords)
        {
            base.UpdatePosition(lastCoords, newCoords);
            MessageBus.Instance.PostEvent(MessageType.PlayerMoved, this, new EventArgs());
        }
    }
}
