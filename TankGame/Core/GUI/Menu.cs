using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Window;
using TankGame.Actors;
using TankGame.Actors.Background;
using TankGame.Actors.Buttons;
using TankGame.Actors.Text;
using TankGame.Core.Controls;
using TankGame.Core.Gamestate;
using TankGame.Core.Statistics;
using TankGame.Events;
using TankGame.Gui.RenderComponents;

namespace TankGame.Core.GUI;

public class Menu : IDisposable {
    public Menu() {
        MenuBackground = new();

        ScoreBox = new(new(0, 350), new(1000, 40), "ffff", 20);
        PlayerNameInput = new(new(300, 410), new(400, 80), "Enter your name", 20);

        QuitButton = new(new(0, 510), new(1000, 80), "Quit", 30, OnQuitClicked);
        QuitNoSaveButton = new(new(0, 610), new(1000, 80), "Surrender", 30, OnQuitNoSaveButton);

        BackButton = new(new(0, 910), new(1000, 80), "Back", 30, OnBackClicked);

        ScoresText = new(new(0, 110), new(1000, 40), "Top Scores", 30);
        ScoresPrev = new(new(100, 810), new(300, 80), "Prev", 30, OnPrevClicked);
        ScoresNext = new(new(600, 810), new(300, 80), "Next", 30, OnNextClicked);

        ScoreActors = new() { BackButton, ScoresText, ScoresNext, ScoresPrev };

        Layers = new() {
            {
                MenuLayer.Main, new() {
                    new ActionTextButton(new(0, 110), new(1000, 80), "New Game", 30, OnStartGameClicked),
                    new ActionTextButton(new(0, 210), new(1000, 80), "Continue", 30, OnContinueGameClicked),
                    new ActionTextButton(new(0, 310), new(1000, 80), "Keyboard Settings", 30, OnKeysClicked),
                    new ActionTextButton(new(0, 410), new(1000, 80), "Top Scores", 30, OnScoresClicked),
                    QuitButton,
                    QuitNoSaveButton,
                }
            }, {
                MenuLayer.Keys, new() {
                    new MenuTextBox(new(100, 110), new(300, 80), InputAction.MoveForward.ToString(), 20, null, TextPosition.Start),
                    new MenuTextBox(new(100, 210), new(300, 80), InputAction.MoveBackwards.ToString(), 20, null, TextPosition.Start),
                    new MenuTextBox(new(100, 310), new(300, 80), InputAction.RotateLeft.ToString(), 20, null, TextPosition.Start),
                    new MenuTextBox(new(100, 410), new(300, 80), InputAction.RotateRight.ToString(), 20, null, TextPosition.Start),
                    new MenuTextBox(new(100, 510), new(300, 80), InputAction.Shoot.ToString(), 20, null, TextPosition.Start),
                    new MenuTextBox(new(100, 610), new(300, 80), InputAction.Pause.ToString(), 20, null, TextPosition.Start),
                    new ChangeKeyButton(new(600, 110), new(300, 80), InputAction.MoveForward, 20, TextPosition.Start),
                    new ChangeKeyButton(new(600, 210), new(300, 80), InputAction.MoveBackwards, 20, TextPosition.Start),
                    new ChangeKeyButton(new(600, 310), new(300, 80), InputAction.RotateLeft, 20, TextPosition.Start),
                    new ChangeKeyButton(new(600, 410), new(300, 80), InputAction.RotateRight, 20, TextPosition.Start),
                    new ChangeKeyButton(new(600, 510), new(300, 80), InputAction.Shoot, 20, TextPosition.Start),
                    new ChangeKeyButton(new(600, 610), new(300, 80), InputAction.Pause, 20, TextPosition.Start),
                    BackButton,
                }
            }, {
                MenuLayer.EndScreen, new() {
                    new MenuTextBox(new(0, 110), new(1000, 80), "GAME OVER", 40),
                    new MenuTextBox(new(0, 310), new(1000, 40), "Your Score:", 20),
                    ScoreBox,
                    PlayerNameInput,
                    new ActionTextButton(new(0, 710), new(1000, 80), "Return to menu", 30, OnBackToMenuClicked),
                }
            },
            { MenuLayer.Scores, new(ScoreActors) },
        };

        ShowMenu();
    }

    private Dictionary<MenuLayer, List<Actor>> Layers { get; }

    private MenuBackground MenuBackground { get; }

    private ActionTextButton QuitButton { get; }
    private ActionTextButton QuitNoSaveButton { get; }

    private MenuTextBox ScoreBox { get; }
    private TextInput PlayerNameInput { get; }

    private ActionTextButton BackButton { get; }

    private MenuTextBox ScoresText { get; }
    private ActionTextButton ScoresPrev { get; }
    private ActionTextButton ScoresNext { get; }

    private List<Actor> ScoreActors { get; }

    private int ScoresOffset { get; set; }

    public string PlayerName => PlayerNameInput.Text;

    public void Dispose()
        => MenuBackground.Dispose();

    public void ShowMenu()
        => ShowLayer(MenuLayer.Main);

    public void ShowEndScreen()
        => ShowLayer(MenuLayer.EndScreen);

    public void Hide() {
        MenuBackground.Visible = false;
        Layers.Values.ToList()
              .ForEach(layer => layer.ForEach(renderable => renderable.Visible = false));
    }

    private void ShowLayer(MenuLayer layer) {
        Refresh(1, layer);

        Hide();
        MenuBackground.Visible = true;

        Refresh(2, layer);

        Layers[layer]
           .ForEach(renderable => renderable.Visible = true);

        Refresh(3, layer);
    }

    private void Refresh(int phase, MenuLayer layer) {
        switch (phase) {
            case 1:
                ScoreBox.SetText(GamestateManager.Points.ToString());
                break;

            case 2:
                Layers[MenuLayer.Scores]
                   .FindAll(actor => !ScoreActors.Contains(actor))
                   .ForEach(
                        actor => {
                            actor.Dispose();
                            Layers[MenuLayer.Scores]
                               .Remove(actor);
                        }
                    );

                var i = 0;

                foreach (Score score in ScoreManager.SkipTake(10, ScoresOffset))
                    Layers[MenuLayer.Scores]
                       .AddRange(new List<Actor> { new MenuTextBox(new(100, 210 + i * 50), new(300, 50), score.Name, 20), new MenuTextBox(new(400, 210 + i++ * 50), new(300, 50), score.Points.ToString(), 20) });

                if (layer != MenuLayer.Scores)
                    Layers[MenuLayer.Scores]
                       .ForEach(actor => actor.Visible = false);
                break;

            case 3:
                QuitNoSaveButton.Visible = layer == MenuLayer.Main   && GamestateManager.GamePhase != GamePhase.NotStarted && GamestateManager.GamePhase != GamePhase.Ending;
                ScoresPrev.Visible = layer       == MenuLayer.Scores && ScoresOffset               > 0;
                ScoresNext.Visible = layer       == MenuLayer.Scores && ScoresOffset + 10          < ScoreManager.GetScoresCount();
                break;
        }
    }

    private void OnStartGameClicked(MouseButtonEventArgs mouseButtonEventArgs)
        => MessageBus.StartGame.Invoke(false);

    private void OnContinueGameClicked(MouseButtonEventArgs mouseButtonEventArgs)
        => MessageBus.StartGame.Invoke(true);

    private void OnQuitClicked(MouseButtonEventArgs mouseButtonEventArgs)
        => MessageBus.Quit.Invoke();

    private void OnBackToMenuClicked(MouseButtonEventArgs mouseButtonEventArgs)
        => MessageBus.StopGame.Invoke();

    private void OnBackClicked(MouseButtonEventArgs mouseButtonEventArgs)
        => ShowLayer(MenuLayer.Main);

    private void OnKeysClicked(MouseButtonEventArgs mouseButtonEventArgs)
        => ShowLayer(MenuLayer.Keys);

    private void OnScoresClicked(MouseButtonEventArgs mouseButtonEventArgs)
        => ShowLayer(MenuLayer.Scores);

    private void OnQuitNoSaveButton(MouseButtonEventArgs obj) {
        if (GamestateManager.Player != null) GamestateManager.Player.OnDestroy();
    }

    private void OnNextClicked(MouseButtonEventArgs obj) {
        ScoresOffset += 10;
        OnScoresClicked(obj);
    }

    private void OnPrevClicked(MouseButtonEventArgs obj) {
        ScoresOffset -= 10;
        OnScoresClicked(obj);
    }
}