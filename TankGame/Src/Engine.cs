using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using TankGame.Src.Actors;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Actors.GameObjects;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Actors.Projectiles;
using TankGame.Src.Actors.Text;
using TankGame.Src.Actors.Weathers;
using TankGame.Src.Data;
using TankGame.Src.Data.Map;
using TankGame.Src.Events;
using TankGame.Src.Extensions;
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
        private CollisionManager CollisionManager { get; set; }

        private HashSet<ITickable> Tickables { get; }
        private HashSet<IRenderable> Renderables { get; }

        private GameMap Map { get; set; }

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

            StartGame(false);
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
            foreach (ITickable tickable in Tickables.ToList())
            {
                tickable.Tick(deltaTime);
            }

            CollisionManager.Tick();
            GamestateManager.Instance.Tick(deltaTime);
        }

        private void Render(float deltaTime)
        {
            Window.DispatchEvents();
            Window.SetView(GameView);
            Window.Clear(Color.Black);

            if (GamestateManager.Instance.Player != null) RecenterView(GamestateManager.Instance.Player.RealPosition);

            PrepareRenderQueue().ForEach((List<IRenderable> RenderLayer)
                => RenderLayer.ForEach((IRenderable renderable)
                    => renderable.GetRenderComponents().ToList().ForEach((IRenderComponent renderComponent)
                        => Window.Draw(renderComponent.Shape, new RenderStates(shader: (renderable is IShadable shadable ? shadable.CurrentShader : null))))));

            Window.Display();
        }

        private List<List<IRenderable>> PrepareRenderQueue()
        {
            List<List<IRenderable>> RenderQueue = new List<List<IRenderable>>
            {
                new List<IRenderable>(),
                new List<IRenderable>(),
                new List<IRenderable>(),
                new List<IRenderable>(),
                new List<IRenderable>(),
                new List<IRenderable>()
            };

            Renderables.ToList().ForEach(renderable =>
            {
                switch (renderable)
                {
                    case TextBox _:
                        RenderQueue[5].Add(renderable);
                        break;
                    case Weather _:
                        RenderQueue[4].Add(renderable);
                        break;
                    case Projectile _:
                        RenderQueue[3].Add(renderable);
                        break;
                    case Pawn _:
                        RenderQueue[2].Add(renderable);
                        break;
                    case GameObject _:
                        RenderQueue[1].Add(renderable);
                        break;
                    case Field _:
                        RenderQueue[0].Add(renderable);
                        break;
                    default:
                        break;
                }
            });

            return RenderQueue;
        }

        private void StartGame(bool isNewGame)
        {
            GamestateManager.Instance.Map = new GameMap(isNewGame);
            Map = GamestateManager.Instance.Map;
        }

        private void InitializeManagers()
        {
            CollisionManager = new CollisionManager();
            TextureManager.Initialize();
            SoundManager.Initialize();
            KeyManager.Initialize();
            MusicManager.Initialize();
        }

        private void InitializeWindow()
        {
            Height = 800;
            Width = Height * 16 / 10;

            Window = new RenderWindow(new VideoMode(Width, Height), GameTitle, Styles.Default, new ContextSettings() { AntialiasingLevel = 8 });
            Window.SetVerticalSyncEnabled(true);
            Window.Closed += (_, __) => Window.Close();
        }

        private void InitializeView()
        {
            ViewWidth = 64 * (2 * 6 + 1);
            ViewHeight = ViewWidth;

            GameView = new View(new Vector2f(ViewWidth / 2, ViewHeight / 2), new Vector2f(ViewWidth, ViewHeight));

            RecalculateViewport(Height, Width);
        }

        private void SetInputHandlers()
        {
            if (Window != null && InputHandler != null)
            {
                Window.KeyPressed += InputHandler.OnKeyPress;
                Window.MouseButtonPressed += InputHandler.OnClick;
                Window.Resized += OnResize;
            }
        }

        private void OnResize(object sender, SizeEventArgs newSize)
        {
            Height = newSize.Height;
            Width = newSize.Width;
            RecalculateViewport(newSize.Height, newSize.Width);
        }

        private void RecalculateViewport(uint height, uint width)
        {
            float aspectRatio = (float)height / width;

            if (GameView != null && (aspectRatio < 0.9 || aspectRatio > 1.1))
            {
                GameView.Viewport = Window.Size.X > Window.Size.Y
                    ? new FloatRect(new Vector2f((1 - aspectRatio) / 2, 0),     new Vector2f(aspectRatio, 1))
                    : new FloatRect(new Vector2f(0, (1 - 1 / aspectRatio) / 2), new Vector2f(1, 1 / aspectRatio));

                GameView.Size = new Vector2f(ViewWidth, ViewHeight);
            }
        }

        private void RecenterView(Vector2f position)
        {
            if (GameView != null) GameView.Center = position;
        }

        private void RegisterEvents()
        {
            MessageBus messageBus = MessageBus.Instance;

            messageBus.Register(MessageType.Quit, OnQuit);
            messageBus.Register(MessageType.RegisterTickable, OnRegisterTickable);
            messageBus.Register(MessageType.UnregisterTickable, OnUnregisterTickable);
            messageBus.Register(MessageType.RegisterRenderable, OnRegisterRenderable);
            messageBus.Register(MessageType.UnregisterRenderable, OnUnregisterRenderable);
            messageBus.Register(MessageType.PlayerMoved, OnPlayerMoved);
        }

        private void OnQuit(object sender, EventArgs eventArgs) => ShouldQuit = true;

        private void OnRegisterTickable(object sender, EventArgs eventArgs)
        {
            if (sender is ITickable tickable) Tickables.Add(tickable);
        }

        private void OnUnregisterTickable(object sender, EventArgs eventArgs)
        {
            if (sender is ITickable tickable) Tickables.Remove(tickable);
        }

        private void OnRegisterRenderable(object sender, EventArgs eventArgs)
        {
            if (sender is IRenderable renderable) Renderables.Add(renderable);
        }

        private void OnUnregisterRenderable(object sender, EventArgs eventArgs)
        {
            if (sender is IRenderable renderable) Renderables.Remove(renderable);
        }
        
        private void OnPlayerMoved(object sender, EventArgs eventArgs)
        {
            if (sender is Player senderPlayer)
            {
                RecenterView(senderPlayer.RealPosition);
            }
        }
    }
}