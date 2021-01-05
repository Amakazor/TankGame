using System;
using TankGame.Src.Actors;
using TankGame.Src.Actors.Shaders;
using TankGame.Src.Actors.Weathers;
using TankGame.Src.Events;

namespace TankGame.Src.Data
{
    internal class WeatherController : ITickable, IDisposable
    {
        public static int WeatherMinimalTime = 30;
        public static int WeatherMaximalTime = 61;
        public static float WeatherMinimalIntensity = 0.5F;
        public static float WeatherMaximalIntensity = 3F;
        private Weather Weather { get; set; }
        public float CurrentWeatherTime { get; private set; }
        public AnimationType AnimationType;

        public WeatherController()
        {
            RegisterTickable();
            CurrentWeatherTime = 0;

            AnimationType = WeatherShader.CanUseShader ? AnimationType.Shaded : AnimationType.Animated;

            GetNewWeather();
        }


        public float GetSpeedModifier()
        {
            return Weather != null ? Weather.SpeedModifier : 1;
        }

        public void Tick(float deltaTime)
        {
            CurrentWeatherTime -= deltaTime;
            if (CurrentWeatherTime <= 0) GetNewWeather();
        }

        private void GetNewWeather()
        {
            if (Weather != null) Weather.Dispose();
            Weather = null;
            CurrentWeatherTime = GamestateManager.Instance.Random.Next(WeatherMinimalTime, WeatherMaximalTime);
            float intensity = (float)((GamestateManager.Instance.Random.NextDouble() * (WeatherMaximalIntensity - WeatherMinimalIntensity)) + WeatherMinimalIntensity);
            Weather = GamestateManager.Instance.Random.Next(1, 4) switch
            {
                1 => null,
                2 => new Weather(TextureManager.Instance.GetTexture(TextureType.Weather, "rain"), 1.15F, MusicType.Rain, intensity, AnimationType),
                3 => new Weather(TextureManager.Instance.GetTexture(TextureType.Weather, "snow"), 1.3F, MusicType.Snow, intensity, AnimationType),
                _ => null,
            };

            if (Weather == null) MusicManager.Instance.StopMusic();
        }

        public void RegisterTickable()
        {
            MessageBus.Instance.PostEvent(MessageType.RegisterTickable, this, new EventArgs());
        }

        public void UnregisterTickable()
        {
            MessageBus.Instance.PostEvent(MessageType.UnregisterTickable, this, new EventArgs());
        }

        public void Dispose()
        {
            UnregisterTickable();
        }
    }
}
