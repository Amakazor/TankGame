using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Actors.Shaders;
using TankGame.Core.Sounds;
using TankGame.Events;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Weathers;

public class Weather : Actor, ITickable, IRenderable {
    public Weather(Texture weatherTexture, float speedModifier, MusicType musicType, float intensity, AnimationType animationType, WeatherType type) : base(animationType == AnimationType.Shaded ? new(0, 0) : new Vector2f(-64, -64), new(64, 64)) {
        SpeedModifier = speedModifier;
        Intensity = intensity;
        AnimationType = animationType;
        Type = type;
        ComplexShader = AnimationType == AnimationType.Shaded ? new WeatherShader(Intensity) : null;

        weatherTexture.Repeated = true;

        PositionOffset = new(0, 0);

        WeatherComponent = new(Position, Size, weatherTexture, new(255, 255, 255, 255));
        WeatherComponent.SetTextureRectSize(new(5 * 20 * 64 + (animationType == AnimationType.Animated ? 2 * 64 : 0), 5 * 20 * 64 + (animationType == AnimationType.Animated ? 2 * 64 : 0)));

        MusicManager.PlayRandom(musicType);

        RenderLayer = RenderLayer.Weather;
        RenderView = RenderView.Game;

        RegisterTickable();
    }

    private ComplexShader? ComplexShader { get; }
    protected SpriteComponent WeatherComponent { get; }
    public float SpeedModifier { get; }
    public float Intensity { get; }
    public AnimationType AnimationType { get; }
    public WeatherType Type { get; }
    public Vector2f PositionOffset { get; set; }
    public Shader? CurrentShader => ComplexShader?.Shader;

    public override HashSet<IRenderComponent> RenderComponents => new() { WeatherComponent };

    public void Tick(float deltaTime) {
        if (AnimationType == AnimationType.Animated) {
            if (PositionOffset.X > 64) PositionOffset -= new Vector2f(64, 64);
            PositionOffset += new Vector2f(Intensity * deltaTime * 64, Intensity * deltaTime * 64);
            WeatherComponent?.SetPosition(Position + PositionOffset);
        }
    }

    public void RegisterTickable()
        => MessageBus.RegisterTickable.Invoke(this);

    public void UnregisterTickable()
        => MessageBus.UnregisterTickable.Invoke(this);

    public override void Dispose() {
        GC.SuppressFinalize(this);

        UnregisterTickable();
        ComplexShader?.Dispose();
        base.Dispose();
    }
}