using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using TankGame.Src.Actors;
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
        private HashSet<ITickable> Tickables { get; }

        public Engine()
        {
            GameTitle = "Tank Game";
            ShouldQuit = false;
            Tickables = new HashSet<ITickable>();

            InitializeManagers();
            InitializeWindow();
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
                Render(deltaTime);

                time1 = time2;
            }
        }

        private void Tick(float deltaTime)
        {
            foreach (ITickable tickable in Tickables)
            {
                tickable.Tick(deltaTime);
            }
        }

        private void Render(float deltaTime)
        {
            Window.DispatchEvents();
            Window.Clear(Color.White);
            Window.Display();
        }

        private void RegisterEvents()
        {
            MessageBus.Instance.Register(MessageType.Quit, OnQuit);
            MessageBus.Instance.Register(MessageType.RegisterTickable, OnRegisterTickable);
            MessageBus.Instance.Register(MessageType.UnregisterTickable, OnUnregisterTickable);
        }
        private void OnQuit(object sender, EventArgs eventArgs)
        {
            ShouldQuit = true;
        }

        private void OnRegisterTickable(object sender, EventArgs eventArgs)
        {
            if (sender is ITickable)
            {
                Tickables.Add((ITickable)sender);
            }
        }

        private void OnUnregisterTickable(object sender, EventArgs eventArgs)
        {
            if (sender is ITickable && Tickables.Contains((ITickable)sender))
            {
                Tickables.Remove((ITickable)sender);
            }
        }

        private void InitializeManagers()
        {
            TextureManager.Initialize();
            SoundManager.Initialize();
            KeyManager.Initialize();
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
