using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Data;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.GUI
{
    class HealthDisplay : Actor
    {
        private RectangleComponent Outline { get; }
        private RectangleComponent Inside { get; }
        private SpriteComponent Heart { get; }

        private readonly HashSet<IRenderComponent> RenderComponents;
        public HealthDisplay() : base(new Vector2f((1000 - (64 * 11 + 16)) / 2, 10), new Vector2f(64 * 11 + 16, 64))
        {
            MessageBus.Instance.Register(MessageType.PlayerHealthChanged, OnPlayerHealthChanged);

            Heart = new SpriteComponent(Position, new Vector2f(64, 64), TextureManager.Instance.GetTexture("hp", "hp"), new Color(255, 255, 255, 255));
            Outline = new RectangleComponent(Position + new Vector2f(48, -16), new Vector2f(64 * 10, 32), new Color(32, 0, 0, 255), new Color(64, 64, 64, 255), 2);
            Inside = new RectangleComponent(Position + new Vector2f(48, -16), new Vector2f(64 * 10, 32), new Color(192, 0, 0, 255));


            RenderComponents = new HashSet<IRenderComponent> { Heart, Outline, Inside };

            RenderLayer = RenderLayer.HUDFront;
            RenderView = RenderView.HUD;
        }

        public override void Dispose()
        {
            MessageBus.Instance.Unregister(MessageType.PlayerHealthChanged, OnPlayerHealthChanged);
            base.Dispose();
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return RenderComponents;
        }

        public void OnPlayerHealthChanged(object sender, EventArgs eventArgs)
        {
            if (eventArgs is PlayerHealthChangeEventArgs playerHealthChange) Inside.SetSize(new Vector2f(64 * playerHealthChange.CurrentHealth, 32));
        }
    }
}
