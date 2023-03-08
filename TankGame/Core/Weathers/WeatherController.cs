using System;
using TankGame.Actors;
using TankGame.Actors.Shaders;
using TankGame.Actors.Weathers;
using TankGame.Core.Sounds;
using TankGame.Core.Textures;
using TankGame.Events;

namespace TankGame.Core.Weathers;

public class WeatherController : ITickable, IDisposable {
    private static readonly Random Random = new();
    
    private const int WeatherMinimalTime = 30;
    private const int WeatherMaximalTime = 61;
    private const float WeatherMinimalIntensity = 0.5F;
    private const float WeatherMaximalIntensity = 3F;
    private readonly AnimationType _animationType;

    public WeatherController() {
        RegisterTickable();
        CurrentWeatherTime = 0;

        _animationType = WeatherShader.CanUseShader ? AnimationType.Shaded : AnimationType.Animated;

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
        MusicManager.Stop();

        CurrentWeatherTime = weatherTime;
        var intensity = (float)(Random.NextDouble() * (WeatherMaximalIntensity - WeatherMinimalIntensity) + WeatherMinimalIntensity);
        Weather = type switch {
            WeatherType.Clear => null,
            WeatherType.Rain  => new(TextureManager.Get(TextureType.Weather, "rain"), 1.15F, MusicType.Rain, intensity, _animationType, type),
            WeatherType.Snow  => new(TextureManager.Get(TextureType.Weather, "snow"), 1.3F, MusicType.Snow, intensity, _animationType, type),
            _                 => null,
        };
    }

    public float GetSpeedModifier()
        => Weather?.SpeedModifier ?? 1;

    private void GetNewWeather() {
        Weather?.Dispose();
        Weather = null;
        CurrentWeatherTime = Random.Next(WeatherMinimalTime, WeatherMaximalTime);
        var intensity = (float)(Random.NextDouble() * (WeatherMaximalIntensity - WeatherMinimalIntensity) + WeatherMinimalIntensity);
        Weather = (WeatherType)Random.Next(0, 3) switch {
            WeatherType.Clear => null,
            WeatherType.Rain  => new(TextureManager.Get(TextureType.Weather, "rain"), 1.15F, MusicType.Rain, intensity, _animationType, WeatherType.Rain),
            WeatherType.Snow  => new(TextureManager.Get(TextureType.Weather, "snow"), 1.3F, MusicType.Snow, intensity, _animationType, WeatherType.Snow),
            _                 => null,
        };

        if (Weather == null) MusicManager.Stop();
    }
}