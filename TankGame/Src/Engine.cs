using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using TankGame.Src.Actors;
using TankGame.Src.Data;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src
{
    internal class Engine
    {
        public string GameTitle { get; private set; }
        private bool ShouldQuit { get; set; }

        private uint Width { get; set; }
        private uint Height { get; set; }
        private RenderWindow Window { get; set; }

        private uint ViewWidth { get; set; }
        private uint ViewHeight { get; set; }
        private View GameView { get; set; }

        private InputHandler InputHandler { get; }

        private HashSet<ITickable> Tickables { get; }
        private HashSet<IRenderable> Renderables { get; }

        public Engine()
        {
            GameTitle = "Tank Game";
            ShouldQuit = false;
            Tickables = new HashSet<ITickable>();
            Renderables = new HashSet<IRenderable>();

            InitializeManagers();

            InitializeWindow();
            InitializeView();

            InputHandler = new InputHandler(Window);
            SetInputHandlers();

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
            Window.SetView(GameView);
            Window.Clear(Color.Black);

            foreach (IRenderable renderable in Renderables)
            {
                foreach (IRenderComponent renderComponent in renderable.GetRenderComponents())
                {
                    Window.Draw(renderComponent.GetShape());
                }
            }

            Window.Display();
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

        private void InitializeView()
        {
            ViewWidth = 600;
            ViewHeight = 600;

            GameView = new View(new Vector2f(Width / 2, Height / 2), new Vector2f(Width, Height));
        }

        private void SetInputHandlers()
        {
            if (Window != null && InputHandler != null)
            {
                Window.KeyPressed += InputHandler.OnKeyPress;
                Window.MouseButtonPressed += InputHandler.OnClick;
            }
        }

        private void RegisterEvents()
        {
            MessageBus messageBus = MessageBus.Instance;

            messageBus.Register(MessageType.Quit, OnQuit);
            messageBus.Register(MessageType.RegisterTickable, OnRegisterTickable);
            messageBus.Register(MessageType.UnregisterTickable, OnUnregisterTickable);
            messageBus.Register(MessageType.RegisterRenderable, OnRegisterRenderable);
            messageBus.Register(MessageType.UnregisterRenderable, OnUnregisterRenderable);
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

        private void OnRegisterRenderable(object sender, EventArgs eventArgs)
        {
            if (sender is IRenderable)
            {
                Renderables.Add((IRenderable)sender);
            }
        }

        private void OnUnregisterRenderable(object sender, EventArgs eventArgs)
        {
            if (sender is IRenderable && Renderables.Contains((IRenderable)sender))
            {
                Renderables.Remove((IRenderable)sender);
            }
        }
    }
}