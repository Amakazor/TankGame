﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TankGame.Actors;
using TankGame.Actors.Data;
using TankGame.Actors.Pawns;
using TankGame.Actors.Pawns.Players;
using TankGame.Core.Collisions;
using TankGame.Core.Controls;
using TankGame.Core.Gamestates;
using TankGame.Core.GUI;
using TankGame.Core.Sounds;
using TankGame.Core.Statistics;
using TankGame.Core.Textures;
using TankGame.Events;

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

        if (Window.IsOpen || Gamestate.NotStarted) return;
        Gamestate.Save();
        Clear();
        Hud.Dispose();
    }

    private void StopGame() {
        Gamestate.DeleteSave();

        if (Gamestate.Ending)
            ScoreManager.Add(new(Menu.PlayerName, Gamestate.Points));
        else
            Gamestate.Save();

        Clear();
        Menu.ShowMenu();
    }

    private void Clear() {
        Gamestate.Clear();
        CollisionHandler.Clear();
        MusicManager.Stop();
    }

    private void Tick(float deltaTime) {
        if (!Gamestate.Playing) return;

        foreach (ITickable tickable in Tickables.ToImmutableList()) tickable.Tick(deltaTime);

        CollisionHandler.Tick();
        Gamestate.Tick(deltaTime);
    }

    private void Render() {
        Window.DispatchEvents();
        Window.Clear(Color.Black);

        if (!Gamestate.NotStarted) {
            foreach (RenderView view in Views.Select(view => view.Key)) RenderInView(view);
        } else {
            RenderInView(RenderView.Menu);
            RenderInView(RenderView.Console);
        }

        Window.Display();
    }

    private void RenderInView(RenderView usedView) {
        Window.SetView(Views[usedView]);
        var components = Renderables
            .Where(renderable => renderable.Visible && renderable.RenderableRenderView == usedView)
            .OrderBy(renderable => renderable.RenderableRenderLayer)
            .SelectMany(renderable => renderable.RenderComponents.Select(component => (renderable.CurrentShader, component.Shape)));

        foreach ((Shader? CurrentShader, Drawable Shape) renderable in components) Window.Draw(renderable.Shape, new(renderable.CurrentShader));
    }

    private void StartGame(bool continueGame) {
        if (Gamestate.Paused && continueGame) {
            Unpause();
            return;
        }

        if (!Gamestate.NotStarted && !continueGame) {
            StopGame();
        }

        Menu.Hide();
        Gamestate.Start(continueGame);
    }
    

    private void Pause() {
        Menu.ShowMenu();
        Gamestate.Pause();
    }

    private void Unpause() {
        Menu.Hide();
        Gamestate.Play();
    }

    private RenderWindow InitializeWindow() {
        WindowWidth = 800;
        WindowHeight = WindowWidth * 16 / 10;

        RenderWindow window = new(new(WindowHeight, WindowWidth), GameTitle, Styles.Default, new() { AntialiasingLevel = 2 });
        window.SetVerticalSyncEnabled(true);

        Texture icon = TextureManager.Get(TextureType.Pawn, "player1");
        window.SetIcon(icon.Size.X, icon.Size.Y, icon.CopyToImage().Pixels);

        window.Closed += (_, _) => window.Close();
        return window;
    }

    private void InitializeView() {
        const int  visibleFields  = 13;
        const float gameViewWidth = 64 * visibleFields;

        const float hudViewWidth  = 1000;
        const float hudViewHeight = hudViewWidth / 5;

        const float menuViewWidth = 1000;

        Views.Add(RenderView.Game,    new(new(gameViewWidth / 2, gameViewWidth / 2), new(gameViewWidth, gameViewWidth)));
        Views.Add(RenderView.HUD,     new(new(hudViewWidth  / 2 - 32, hudViewHeight / 2 - 32), new(hudViewWidth, hudViewHeight)));
        Views.Add(RenderView.Menu,    new(new(menuViewWidth / 2, menuViewWidth / 2), new(menuViewWidth, menuViewWidth)));
        Views.Add(RenderView.Console, new(new(menuViewWidth / 2, menuViewWidth / 2), new(menuViewWidth, menuViewWidth)));

        RecalculateViewport(WindowWidth, WindowHeight);
    }

    private void SetInputHandlers() {
        Window.Resized += (_, args) => OnResize(args);
        Window.KeyPressed += (_, args) => InputHandler.OnKeyPress(args);
        Window.MouseButtonPressed += (_, args) => InputHandler.OnClick(args);
        Window.TextEntered += (_, args) => InputHandler.OnTextInput(args);
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

        if (Views.TryGetValue(RenderView.HUD, out View? hudView))  hudView!.Viewport  = Window.Size.X > Window.Size.Y ? new(new((1 - uiWidth * aspectRatio) / 2, 0), new(uiWidth * aspectRatio, uiHeight)) : new FloatRect(new((1 - uiWidth) / 2, 0), new(uiWidth, uiHeight * (1 / aspectRatio)));

        if (Views.TryGetValue(RenderView.Menu, out View? menuView)) menuView!.Viewport = Window.Size.X > Window.Size.Y ? new(new((1 - aspectRatio) / 2, 0), new(aspectRatio, 1)) : new FloatRect(new(0, (1 - 1 / aspectRatio) / 2), new(1, 1 / aspectRatio));
    }

    private void RecenterView(Vector2f position) {
        if (Views.TryGetValue(RenderView.Game, out View? gameView)) {
            gameView!.Center = position;
            Window.SetView(gameView);
        }
    }

    private void RegisterEvents() {
        MessageBus.RegisterTickable += sender => Tickables.Add(sender);
        MessageBus.UnregisterTickable += sender => Tickables.Remove(sender);

        MessageBus.RegisterRenderable += sender => Renderables.Add(sender);
        MessageBus.UnregisterRenderable += sender => Renderables.Remove(sender);

        MessageBus.StartGame += StartGame;
        MessageBus.StopGame += StopGame;
        MessageBus.Quit += Quit;

        MessageBus.KeyAction += OnAction;

        MessageBus.PlayerMoved += sender => RecenterView(sender.Position);
        MessageBus.PawnDeath += OnPawnDeath;
    }

    private void Quit() {
        if (Gamestate.GamePhase != GamePhase.NotStarted)
            StopGame();
        else
            ShouldQuit = true;
    }

    private void OnPawnDeath(Pawn sender) {
        if (sender is not Player) return;

        Gamestate.End();
        Menu.ShowEndScreen();
    }

    private void OnAction(InputAction inputAction) {
        switch (inputAction) {
            case InputAction.Pause when Gamestate.Playing:
                Pause();
                break;
            case InputAction.Pause when Gamestate.Paused:
                Unpause();
                break;
        }
    }
}