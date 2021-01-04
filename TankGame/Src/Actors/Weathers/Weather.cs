using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using TankGame.Src.Actors.Shaders;
using TankGame.Src.Data;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Weathers
{
    internal class Weather : ShadedActor
    {
        public override Shader CurrentShader => ComplexShader?.Shader;
        private ComplexShader ComplexShader { get; }
        protected SpriteComponent WeatherComponent { get; }
        public float SpeedModifier { get; protected set; }

        public Weather(Texture weatherTexture, float speedModifier, string musicType, float intensity) : base(new Vector2f(0, 0), new Vector2f(64, 64))
        {
            ComplexShader = Shader.IsAvailable ? new WeatherShader(intensity) : null;

            SpeedModifier = speedModifier;

            weatherTexture.Repeated = true;

            WeatherComponent = new SpriteComponent(Position, Size, this, weatherTexture, new Color(255, 255, 255, 255));
            WeatherComponent.SetTextureRectSize(new Vector2i(3 * 20 * 64, 3 * 20 * 64));

            MusicManager.Instance.PlayRandomMusic(musicType);
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { WeatherComponent };
        }
    }
}
