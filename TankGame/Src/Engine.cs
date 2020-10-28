using SFML.Graphics;
using SFML.Window;
using System;
using TankGame.Src.Data;
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

            InitializeManagers();
            RegisterEvents();
            InitializeWindow();
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
                Render(deltaTime);

                time1 = time2;
            }
        }

        private void Tick(float deltaTime)
        {

        }

        private void Render(float deltaTime)
        {
            Window.DispatchEvents();
            Window.Clear(Color.White);
            Window.Display();
        }

        private void OnQuit(object sender, EventArgs eventArgs)
        {
            ShouldQuit = true;
        }

        private void RegisterEvents()
        {
            MessageBus.Instance.Register(MessageType.Quit, OnQuit);
        }

        private void InitializeManagers()
        {
            TextureManager.Initialize();
        }

        private void InitializeWindow()
        {
            Width = 600;
            Height = 600;

            Window = new RenderWindow(new VideoMode(Width, Height), GameTitle, Styles.Default, new ContextSettings() { AntialiasingLevel = 8 });
            Window.SetVerticalSyncEnabled(true);
            Window.Closed += (_, __) => Window.Close();
        }
    }
}
