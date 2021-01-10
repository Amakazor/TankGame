using SFML.Graphics;

namespace TankGame.Src.Actors
{
    internal interface IShadable
    {
        public Shader CurrentShader { get; }
    }
}