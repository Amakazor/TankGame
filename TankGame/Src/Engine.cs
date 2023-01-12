using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TankGame.Actors;
using TankGame.Actors.Data;
using TankGame.Actors.Pawns;
using TankGame.Actors.Pawns.Player;
using TankGame.Core.Collisions;
using TankGame.Core.Gamestate;
using TankGame.Core.GUI;
using TankGame.Core.Sounds;
using TankGame.Core.Statistics;
using TankGame.Core.Textures;
using TankGame.Events;
using Action = TankGame.Core.Controls.Action;

namespace TankGame;

public class Engine {
    public Engine() {
        Window = InitializeWindow();
        Views = new();
        InitializeView();

        InputHandler = new(Window);
        SetInputHandlers();

        RegisterEvents();

        Menu = new();
        Hud = new();
    }

    private static string GameTitle => "Tank Game";
    private bool ShouldQuit { get; set; }

    private uint WindowHeight { get; set; }
    private uint WindowWidth { get; set; }
    private RenderWindow Window { get; set; }

    private Dictionary<RenderView, View?> Views { get; }

    private InputHandler InputHandler { get; }
    private CollisionHandler CollisionHandler { get; } = new();

    private Hud Hud { get; }
    private Menu Menu { get; }

    private HashSet<ITickable> Tickables { get; } = new();
    private HashSet<IRenderable> Renderables { get; } = new();

    public void Loop() {
        DateTime previousFrameTime = DateTime.Now;

        while (Window.IsOpen && !ShouldQuit) {
            DateTime currentFrameTime = DateTime.Now;
            float deltaTime = (currentFrameTime.Ticks - previousFrameTime.Ticks) / 10000000f;

            Tick(deltaTime);
            Render();

            previousFrameTime = currentFrameTime;
        }

        if (Window.IsOpen || GamestateManager.NotStarted) return;
        GamestateManager.Save();
        Clear();
        Hud.Dispose();
    }

    private void StopGame() {
        GamestateManager.DeleteSave();

        if (GamestateManager.Ending)
            ScoreManager.Add(new(Menu.PlayerName, GamestateManager.Points));
        else
            GamestateManager.Save();

        Clear();
        Menu.ShowMenu();
    }

    private void Clear() {
        GamestateManager.Clear();
        CollisionHandler.Clear();
        MusicManager.StopMusic();
    }

    private void Tick(float deltaTime) {
        if (!GamestateManager.Playing) return;

        foreach (ITickable tickable in Tickables.ToImmutableList()) tickable.Tick(deltaTime);

        CollisionHandler.Tick();
        GamestateManager.Tick(deltaTime);
    }

    private void Render() {
        Window.DispatchEvents();
        Window.Clear(Color.Black);

        IEnumerable<(Shader? CurrentShader, Drawable Shape)>? components;
        if (!GamestateManager.NotStarted) {
            Window.SetView(Views[RenderView.Game]);
            if (GamestateManager.Player != null) RecenterView(GamestateManager.Player.RealPosition);

            components = Renderables.Where(renderable => renderable is { RenderableRenderView: RenderView.Game, Visible: true })
                                    .OrderBy(renderable => renderable.RenderableRenderLayer)
                                    .SelectMany(renderable => renderable.RenderComponents.Select(component => (renderable.CurrentShader, component.Shape)));

            foreach (var renderable in components) Window.Draw(renderable.Shape, new(renderable.CurrentShader));

            Window.SetView(Views[RenderView.HUD]);

            components = Renderables.Where(renderable => renderable is { RenderableRenderView: RenderView.HUD, Visible: true })
                                    .OrderBy(renderable => renderable.RenderableRenderLayer)
                                    .SelectMany(renderable => renderable.RenderComponents.Select(component => (renderable.CurrentShader, component.Shape)));
            
            foreach (var renderable in components)
                Window.Draw(renderable.Shape, new(renderable.CurrentShader));
        }

        Window.SetView(Views[RenderView.Menu]);
        
        components = Renderables.Where(renderable => renderable is { RenderableRenderView: RenderView.Menu, Visible: true })
                                .OrderBy(renderable => renderable.RenderableRenderLayer)
                                .SelectMany(renderable => renderable.RenderComponents.Select(component => (renderable.CurrentShader, component.Shape)));

        foreach (var renderable in components)
            Window.Draw(renderable.Shape, new(renderable.CurrentShader));

        Window.Display();
    }

    private void StartGame(bool continueGame) {
        if (GamestateManager.Paused && continueGame) {
            Unpause();
            return;
        }

        if (!GamestateManager.NotStarted && !continueGame) {
            StopGame();
            return;
        }

        Menu.Hide();

        GamestateManager.Start(continueGame);
    }

    private void Pause() {
        Menu.ShowMenu();
        GamestateManager.Pause();
    }

    private void Unpause() {
        Menu.Hide();
        GamestateManager.Play();
    }

    private RenderWindow InitializeWindow() {
        WindowWidth = 800;
        WindowHeight = WindowWidth * 16 / 10;

        RenderWindow window = new(new(WindowHeight, WindowWidth), GameTitle, Styles.Default, new() { AntialiasingLevel = 2 });
        window.SetVerticalSyncEnabled(true);

        Texture icon = TextureManager.GetTexture(TextureType.Pawn, "player1");
        window.SetIcon(
            icon.Size.X, icon.Size.Y, icon.CopyToImage()
                                          .Pixels
        );

        window.Closed += (_, _) => window.Close();
        return window;
    }

    private void InitializeView() {
        const float gameViewWidth = 64 * (2 * 6 + 1);
        const float gameViewHeight = gameViewWidth;

        const float hudViewWidth = 1000;
        const float hudViewHeight = hudViewWidth / 5;

        const float menuViewWidth = 1000;
        const float menuViewHeight = menuViewWidth; 
        Views.Add(RenderView.Game, new(new(gameViewWidth / 2, gameViewHeight / 2), new(gameViewWidth, gameViewHeight)));
        Views.Add(RenderView.HUD, new(new(hudViewWidth / 2 - 32, hudViewHeight / 2 - 32), new(hudViewWidth, hudViewHeight)));
        Views.Add(RenderView.Menu, new(new(menuViewWidth / 2, menuViewHeight / 2), new(menuViewWidth, menuViewHeight)));

        RecalculateViewport(WindowWidth, WindowHeight);
    }

    private void SetInputHandlers() {
        Window.Resized += (_,            args) => OnResize(args);
        Window.KeyPressed += (_,         args) => InputHandler.OnKeyPress(args);
        Window.MouseButtonPressed += (_, args) => InputHandler.OnClick(args);
        Window.TextEntered += (_,        args) => InputHandler.OnTextInput(args);
    }

    private void OnResize(SizeEventArgs newSize) {
        WindowWidth = newSize.Height;
        WindowHeight = newSize.Width;

        RecalculateViewport(newSize.Height, newSize.Width);
    }

    private void RecalculateViewport(uint height, uint width) {
        const float uiWidth = 1F;
        const float uiHeight = 0.2F;

        const float gameSize = 0.8F;

        float aspectRatio = (float)height / width;

        if (Views.TryGetValue(RenderView.Game, out View? gameView)) gameView!.Viewport = Window.Size.X > Window.Size.Y ? new(new(1 - gameSize + (gameSize - gameSize * aspectRatio) / 2, 1 - gameSize), new(gameSize * aspectRatio, gameSize)) : new FloatRect(new(1 - gameSize, 1 - gameSize + (gameSize - gameSize * (1 / aspectRatio)) / 2), new(gameSize, gameSize * (1 / aspectRatio)));

        if (Views.TryGetValue(RenderView.Game, out View? hudView))  hudView!.Viewport  = Window.Size.X > Window.Size.Y ? new(new((1 - uiWidth * aspectRatio) / 2, 0), new(uiWidth * aspectRatio, uiHeight)) : new FloatRect(new((1 - uiWidth) / 2, 0), new(uiWidth, uiHeight * (1 / aspectRatio)));

        if (Views.TryGetValue(RenderView.Game, out View? menuView)) menuView!.Viewport = Window.Size.X > Window.Size.Y ? new(new((1 - aspectRatio) / 2, 0), new(aspectRatio, 1)) : new FloatRect(new(0, (1 - 1 / aspectRatio) / 2), new(1, 1 / aspectRatio));
    }

    private void RecenterView(Vector2f position) {
        if (Views.TryGetValue(RenderView.Game, out View? gameView)) gameView!.Center = position;
    }

    private void RegisterEvents() {
        MessageBus.RegisterTickable += sender => Tickables.Add(sender);
        MessageBus.UnregisterTickable += sender => Tickables.Remove(sender);

        MessageBus.RegisterRenderable += sender => Renderables.Add(sender);
        MessageBus.UnregisterRenderable += sender => Renderables.Remove(sender);

        MessageBus.StartGame += StartGame;
        MessageBus.StopGame += StopGame;
        MessageBus.Quit += Quit;

        MessageBus.Action += OnAction;

        MessageBus.PlayerMoved += sender => RecenterView(sender.RealPosition);
        MessageBus.PawnDeath += OnPawnDeath;
    }

    private void Quit() {
        if (GamestateManager.GamePhase != GamePhase.NotStarted)
            StopGame();
        else
            ShouldQuit = true;
    }

    private void OnPawnDeath(Pawn sender) {
        if (sender is not Player) return;

        GamestateManager.End();
        Menu.ShowEndScreen();
    }

    private void OnAction(Action action) {
        switch (action) {
            case Action.Pause when GamestateManager.Playing:
                Pause();
                break;
            case Action.Pause when GamestateManager.Paused:
                Unpause();
                break;
        }
    }
}