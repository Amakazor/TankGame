using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Data;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.GUI
{
    internal class ActivityDisplay : Actor, ITickable
    {
        private AlignedTextComponent ActivityName { get; set; }
        private AlignedTextComponent ActivityText { get; set; }

        public ActivityDisplay() : base(new Vector2f(10, 64 + 10), new Vector2f(490, 200 - (64 + 10)))
        {
            ActivityName = new AlignedTextComponent(Position, new Vector2f(Size.X, Size.Y / 3), new Vector2f(0, 0), 17, TextPosition.Start, TextPosition.Middle, "", Color.White);
            ActivityText = new AlignedTextComponent(Position + new Vector2f(0, Size.Y / 3), new Vector2f(Size.X, Size.Y / 3), new Vector2f(0, 0), 15, TextPosition.Start, TextPosition.Middle, "", Color.White);

            RenderView = RenderView.HUD;
            RenderLayer = RenderLayer.HUDFront;

            RegisterTickable();
        }

        private void UpdateActivityDisplay()
        {
            if (GamestateManager.Instance?.Player?.CurrentRegion?.Activity?.ProgressText != null)
            {
                ActivityName.SetText("Current objective: " + GamestateManager.Instance.Player.CurrentRegion.Activity.Name);
                ActivityText.SetText(GamestateManager.Instance.Player.CurrentRegion.Activity.ProgressText ?? "");
            }
            else
            {
                ActivityName.SetText("No current objective.");
                ActivityText.SetText("");
            }
        }

        public override void Dispose()
        {
            UnregisterTickable();
            base.Dispose();
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { ActivityName, ActivityText };
        }

        public void Tick(float deltaTime)
        {
            UpdateActivityDisplay();
        }

        public void RegisterTickable()
        {
            MessageBus.Instance.PostEvent(MessageType.RegisterTickable, this, new EventArgs());
        }

        public void UnregisterTickable()
        {
            MessageBus.Instance.PostEvent(MessageType.UnregisterTickable, this, new EventArgs());
        }
    }
}