using SFML.Graphics;
using SFML.System;

namespace TankGame.Src.Actors
{
    internal abstract class ShadedActor : Actor, IShadable
    {
        public abstract Shader CurrentShader { get; }

        protected ShadedActor(Vector2f position, Vector2f size) : base(position, size)
        {
        }
    }
}