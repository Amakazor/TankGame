using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Actors.Pawn.MovementControllers;
using TankGame.Src.Data;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Pawn.Player
{
    internal class Player : Pawn
    {
        public Player(Vector2f position, Vector2f size) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "player1"))
        {
            MovementController = new PlayerMovementController(0.3F, this);

            MessageBus.Instance.PostEvent(MessageType.PlayerMoved, this, new EventArgs());
        }

        protected override void UpdatePosition()
        {
            base.UpdatePosition();
            MessageBus.Instance.PostEvent(MessageType.PlayerMoved, this, new EventArgs());
        }
    }
}
