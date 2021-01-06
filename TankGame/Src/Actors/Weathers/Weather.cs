using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Actors.Shaders;
using TankGame.Src.Data;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Weathers
{
    internal class Weather : ShadedActor, ITickable
    {
        public override Shader CurrentShader => ComplexShader?.Shader;
        private ComplexShader ComplexShader { get; }
        protected SpriteComponent WeatherComponent { get; set; }
        public float SpeedModifier { get; protected set; }
        public float Intensity { get; }
        public AnimationType AnimationType { get; }
        public string Type { get; }
        public Vector2f PositionOffset { get; set; }

        public Weather(Texture weatherTexture, float speedModifier, string musicType, float intensity, AnimationType animationType, string type) : base(animationType == AnimationType.Shaded ? new Vector2f(0, 0) : new Vector2f(-128, -128), new Vector2f(64, 64))
        {
            SpeedModifier = speedModifier;
            Intensity = intensity;
            AnimationType = animationType;
            Type = type;
            ComplexShader = AnimationType == AnimationType.Shaded ? new WeatherShader(Intensity) : null;

            weatherTexture.Repeated = true;

            PositionOffset = new Vector2f(0, 0);

            WeatherComponent = new SpriteComponent(Position, Size, this, weatherTexture, new Color(255, 255, 255, 255));
            WeatherComponent.SetTextureRectSize(new Vector2i(5 * 20 * 64 + (animationType == AnimationType.Shaded ? 2 * 64 : 0), 5 * 20 * 64 + (animationType == AnimationType.Shaded ? 2 * 64 : 0)));

            MusicManager.Instance.PlayRandomMusic(musicType);

            RenderLayer = RenderLayer.Weather;
            RenderView = RenderView.Game;

            RegisterTickable();
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { WeatherComponent };
        }

        public void Tick(float deltaTime)
        {
            if (AnimationType == AnimationType.Animated)
            {
                if (PositionOffset.X > 64) PositionOffset -= new Vector2f(64, 64);
                PositionOffset += new Vector2f(Intensity * deltaTime * 64, Intensity * deltaTime * 64);
              //  WeatherComponent.SetPosition(Position + PositionOffset);
            }
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
            WeatherComponent = null;
            if (ComplexShader != null) ComplexShader.Dispose();
            base.Dispose();
        }
    }
}
