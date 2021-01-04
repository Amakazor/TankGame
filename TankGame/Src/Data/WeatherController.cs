using SFML.Audio;
using System;
using TankGame.Src.Actors;
using TankGame.Src.Actors.Weathers;
using TankGame.Src.Events;

namespace TankGame.Src.Data
{
    internal class WeatherController : ITickable, IDisposable
    {
        public static int WeatherMinimalTime = 1;
        public static int WeatherMaximalTime = 4;
        public static float WeatherMinimalIntensity = 0.5F;
        public static float WeatherMaximalIntensity = 3F;
        private Weather Weather { get; set; }
        public float CurrentWeatherTime { get; private set; }

        public WeatherController(float newWeatherTime, int WeatherType, float intensity)
        {
            RegisterTickable();
            CurrentWeatherTime = 0;

            GetNewWeather(newWeatherTime, WeatherType, intensity);
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

        private void GetNewWeather(float newWeatherTime = 0, int weatherType = 0, float intensity = 0)
        {
            if (Weather != null) Weather.Dispose();
            Weather = null;
            CurrentWeatherTime = newWeatherTime != 0 ? newWeatherTime : GamestateManager.Instance.Random.Next(WeatherMinimalTime, WeatherMaximalTime);
            if (intensity == 0) intensity = (float)((GamestateManager.Instance.Random.NextDouble() * (WeatherMaximalIntensity - WeatherMinimalIntensity)) + WeatherMinimalIntensity);
            Weather = (weatherType != 0 ? weatherType : GamestateManager.Instance.Random.Next(1, 4)) switch
            {
                1 => null,
                2 => new Weather(TextureManager.Instance.GetTexture(TextureType.Weather, "rain"), 1.15F, MusicType.Rain, intensity),
                3 => new Weather(TextureManager.Instance.GetTexture(TextureType.Weather, "snow"), 1.3F, MusicType.Snow, intensity),
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
