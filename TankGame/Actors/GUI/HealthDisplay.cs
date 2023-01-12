using System;
using System.Collections.Generic;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Core.Textures;
using TankGame.Events;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.GUI;

public class HealthDisplay : Actor {
    public HealthDisplay() : base(new((1000 - (64 * 11 + 16)) / 2, 10), new(64 * 11 + 16, 64)) {
        MessageBus.PlayerHealthChanged += OnPlayerHealthChanged;

        Heart = new(Position, new(64, 64), TextureManager.Get(TextureType.Hp, "hp"), new(255, 255, 255, 255));
        Outline = new(Position + new Vector2f(48, -16), new(64 * 10, 32), new(32, 0, 0, 255), new(64, 64, 64, 255), 2);
        Inside = new(Position  + new Vector2f(48, -16), new(64 * 10, 32), new(192, 0, 0, 255));

        RenderComponents = new() { Heart, Outline, Inside };

        RenderLayer = RenderLayer.HUDFront;
        RenderView = RenderView.HUD;
    }

    private RectangleComponent Outline { get; }
    private RectangleComponent Inside { get; }
    private SpriteComponent Heart { get; }

    public override HashSet<IRenderComponent> RenderComponents { get; }

    public override void Dispose() {
        GC.SuppressFinalize(this);
        MessageBus.PlayerHealthChanged -= OnPlayerHealthChanged;
        base.Dispose();
    }

    public void OnPlayerHealthChanged(int currentHealth)
        => Inside.SetSize(new(64 * currentHealth, 32));
}