using SFML.Graphics;
using SFML.Window;
using System;
using TankGame.Src.Events;

namespace TankGame.Src
{
    class Engine
    {
        public string GameTitle { get; private set; }
        private bool ShouldQuit { get; set; }

        private uint Width { get; set; }
        private uint Height { get; set; }
        private RenderWindow Window { get; set; }

        public Engine()
        {
            GameTitle = "Tank Game";
            ShouldQuit = false;

            Window = new RenderWindow(new VideoMode(Width, Height), GameTitle, Styles.Default, new ContextSettings() { AntialiasingLevel = 8 });
            Window.SetVerticalSyncEnabled(true);
            Window.Closed += (_, __) => Window.Close();

            RegisterEvents();
        }

        public void Loop()
        {
            DateTime time1 = DateTime.Now;
            DateTime time2;

            while (Window.IsOpen && !ShouldQuit)
            {
                time2 = DateTime.Now;
                float deltaTime = (time2.Ticks - time1.Ticks) / 10000000f;

                Tick(deltaTime);
                Render();

                time1 = time2;
            }
        }

        private void Tick(float deltaTime)
        {

        }

        private void Render()
        {

        }

        private void OnQuit(object sender, EventArgs eventArgs)
        {
            ShouldQuit = true;
        }

        private void RegisterEvents()
        {
            MessageBus.Instance.Register(MessageType.Quit, OnQuit);
        }
    }
}
