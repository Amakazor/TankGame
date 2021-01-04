using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankGame.Src.Actors
{
    internal abstract class ShadedActor : Actor, IShadable
    {
        public abstract Shader CurrentShader { get; }

        public ShadedActor(Vector2f position, Vector2f size) : base(position, size)
        {
        }
    }
}
