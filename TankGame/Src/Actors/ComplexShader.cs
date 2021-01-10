using SFML.Graphics;
using System;
using TankGame.Src.Events;

namespace TankGame.Src.Actors.Shaders
{
    internal abstract class ComplexShader : ITickable, IDisposable
    {
        public Shader Shader { get; protected set; }

        protected ComplexShader(Shader shader)
        {
            Shader = shader;
            RegisterTickable();
        }

        public abstract void Tick(float deltaTime);

        public void RegisterTickable() => MessageBus.Instance.PostEvent(MessageType.RegisterTickable, this, new EventArgs());

        public void UnregisterTickable() => MessageBus.Instance.PostEvent(MessageType.UnregisterTickable, this, new EventArgs());

        public void Dispose()
        {
            UnregisterTickable();
        }
    }
}