using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Data;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Pawn.Player
{
    internal class Player : Pawn
    {
        public Player(Vector2f position, Vector2f size) : base(position, size, TextureManager.Instance.GetTexture(TextureType.Pawn, "player1"))
        {
            MessageBus.Instance.PostEvent(MessageType.PlayerMoved, this, new EventArgs());
        }

        public override bool IsAlive()
        {
            throw new NotImplementedException();
        }

        public override void OnDestroy(Actor other)
        {
            throw new NotImplementedException();
        }

        public override void OnHit(Actor other)
        {
            throw new NotImplementedException();
        }
    }
}
