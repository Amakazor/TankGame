using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Events;

namespace TankGame.Src.Actors.Text
{
    internal class PointsAddedTextBox : TextBox, ITickable
    {
        public double TimeToLive { get; private set; }
        public PointsAddedTextBox(Vector2f position, uint points, uint combo) : base(position, new Vector2f(100, 20), points + (combo > 1 ? " X " + combo : ""), 16, Color.Red)
        {
            TimeToLive = 2;
            RegisterTickable();
        }

        public void Tick(float deltaTime)
        {
            TimeToLive -= deltaTime;

            Position += (new Vector2f(0, -32) * deltaTime);

            Text.SetPosition(Position);

            if (TimeToLive <= 0) Dispose();
        }

        public void RegisterTickable()
        {
            MessageBus.Instance.PostEvent(MessageType.RegisterTickable, this, new EventArgs());
        }

        public void UnregisterTickable()
        {
            MessageBus.Instance.PostEvent(MessageType.UnregisterTickable, this, new EventArgs());
        }

        public override void Dispose()
        {
            UnregisterTickable();
            base.Dispose();
        }
    }
}
