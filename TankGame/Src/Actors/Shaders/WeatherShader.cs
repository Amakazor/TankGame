using SFML.Graphics;
using SFML.Graphics.Glsl;

namespace TankGame.Src.Actors.Shaders
{
    internal class WeatherShader : ComplexShader
    {
        private float Time;
        public WeatherShader(float intensity) : base(new Shader(null, null, "resources/shaders/fragment/scroll.glsl"))
        {
            Shader.SetUniform("textureSample", Shader.CurrentTexture);
            Shader.SetUniform("resolution", new Vec2(64, 64));
            Shader.SetUniform("intensityX", -intensity);
            Shader.SetUniform("intensityY", intensity);
        }

        public override void Tick(float deltaTime)
        {
            Time = Time + deltaTime > 320 ? Time - 320 + deltaTime : Time + deltaTime;

            Shader.SetUniform("time", Time);
        }
    }
}
