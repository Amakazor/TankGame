using System;
using TankGame.Actors;
using TankGame.Actors.Shaders;
using TankGame.Actors.Weathers;
using TankGame.Core.Gamestate;
using TankGame.Core.Sounds;
using TankGame.Core.Textures;
using TankGame.Events;

namespace TankGame.Core.Weathers;

public class WeatherController : ITickable, IDisposable {
    public const int WeatherMinimalTime = 30;
    public const int WeatherMaximalTime = 61;
    public const float WeatherMinimalIntensity = 0.5F;
    public const float WeatherMaximalIntensity = 3F;
    public readonly AnimationType AnimationType;

    public WeatherController() {
        RegisterTickable();
        CurrentWeatherTime = 0;

        AnimationType = WeatherShader.CanUseShader ? AnimationType.Shaded : AnimationType.Animated;

        GetNewWeather();
    }

    private Weather? Weather { get; set; }
    public float CurrentWeatherTime { get; private set; }
    public WeatherType WeatherType => Weather is null ? WeatherType.Clear : Weather.Type;

    public void Dispose() {
        GC.SuppressFinalize(this);
        Weather?.Dispose();
        UnregisterTickable();
    }

    public void Tick(float deltaTime) {
        CurrentWeatherTime -= deltaTime;
        if (CurrentWeatherTime <= 0) GetNewWeather();
    }

    public void RegisterTickable()
        => MessageBus.RegisterTickable.Invoke(this);

    public void UnregisterTickable()
        => MessageBus.UnregisterTickable.Invoke(this);

    public void SetWeather(WeatherType type, float weatherTime) {
        Weather?.Dispose();
        Weather = null;
        MusicManager.StopMusic();

        CurrentWeatherTime = weatherTime;
        var intensity = (float)(GamestateManager.Random.NextDouble() * (WeatherMaximalIntensity - WeatherMinimalIntensity) + WeatherMinimalIntensity);
        Weather = type switch {
            WeatherType.Clear => null,
            WeatherType.Rain  => new(TextureManager.GetTexture(TextureType.Weather, "rain"), 1.15F, MusicType.Rain, intensity, AnimationType, type),
            WeatherType.Snow  => new(TextureManager.GetTexture(TextureType.Weather, "snow"), 1.3F, MusicType.Snow, intensity, AnimationType, type),
            _                 => null,
        };
    }

    public float GetSpeedModifier()
        => Weather?.SpeedModifier ?? 1;

    private void GetNewWeather() {
        Weather?.Dispose();
        Weather = null;
        CurrentWeatherTime = GamestateManager.Random.Next(WeatherMinimalTime, WeatherMaximalTime);
        var intensity = (float)(GamestateManager.Random.NextDouble() * (WeatherMaximalIntensity - WeatherMinimalIntensity) + WeatherMinimalIntensity);
        Weather = (WeatherType)GamestateManager.Random.Next(0, 3) switch {
            WeatherType.Clear => null,
            WeatherType.Rain  => new(TextureManager.GetTexture(TextureType.Weather, "rain"), 1.15F, MusicType.Rain, intensity, AnimationType, WeatherType.Rain),
            WeatherType.Snow  => new(TextureManager.GetTexture(TextureType.Weather, "snow"), 1.3F, MusicType.Snow, intensity, AnimationType, WeatherType.Snow),
            _                 => null,
        };

        if (Weather == null) MusicManager.StopMusic();
    }
}