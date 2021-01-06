using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankGame.Src.Actors;
using TankGame.Src.Actors.Background;
using TankGame.Src.Actors.Buttons;
using TankGame.Src.Actors.Pawns.Player;
using TankGame.Src.Actors.Text;
using TankGame.Src.Data.Statistics;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Data
{
    class Menu : IDisposable
    {
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

        private int ScoresOffset { get; set; }

        public string PlayerName => PlayerNameInput.Text;

        public Menu()
        {
            MenuBackground = new MenuBackground();

            ScoreBox = new MenuTextBox(new Vector2f(0, 350), new Vector2f(1000, 40), "ffff", 20);
            PlayerNameInput = new TextInput(new Vector2f(300, 410), new Vector2f(400, 80), "Enter your name", 20);

            QuitButton = new ActionTextButton(new Vector2f(0, 510), new Vector2f(1000, 80), "Quit", 30, OnQuitClicked);
            QuitNoSaveButton = new ActionTextButton(new Vector2f(0, 610), new Vector2f(1000, 80), "Surrender", 30, OnQuitNoSaveButton);

            BackButton = new ActionTextButton(new Vector2f(0, 910), new Vector2f(1000, 80), "Back", 30, OnBackClicked);

            ScoresText = new MenuTextBox(new Vector2f(0, 110), new Vector2f(1000, 40), "Top Scores", 30);
            ScoresPrev = new ActionTextButton(new Vector2f(100, 810), new Vector2f(300, 80), "Prev", 30, OnPrevClicked);
            ScoresNext = new ActionTextButton(new Vector2f(600, 810), new Vector2f(300, 80), "Next", 30, OnNextClicked);


            Layers = new Dictionary<MenuLayer, List<Actor>>
            {
                {
                    MenuLayer.Main,
                    new List<Actor>
                    {
                        new ActionTextButton(new Vector2f(0, 110), new Vector2f(1000, 80), "New Game", 30, OnStartGameClicked),
                        new ActionTextButton(new Vector2f(0, 210), new Vector2f(1000, 80), "Continue", 30, OnContinueGameClicked),
                        new ActionTextButton(new Vector2f(0, 310), new Vector2f(1000, 80), "Keyboard Settings", 30, OnKeysClicked),
                        new ActionTextButton(new Vector2f(0, 410), new Vector2f(1000, 80), "Top Scores", 30, OnScoresClicked),
                        QuitButton,
                        QuitNoSaveButton
                    }
                },
                {
                    MenuLayer.Keys,
                    new List<Actor>
                    {
                        new MenuTextBox(new Vector2f(100, 110), new Vector2f(300, 80), KeyActionType.MoveUp.Item2, 20, null, TextPosition.Start),
                        new MenuTextBox(new Vector2f(100, 210), new Vector2f(300, 80), KeyActionType.MoveDown.Item2, 20, null, TextPosition.Start),
                        new MenuTextBox(new Vector2f(100, 310), new Vector2f(300, 80), KeyActionType.MoveLeft.Item2, 20, null, TextPosition.Start),
                        new MenuTextBox(new Vector2f(100, 410), new Vector2f(300, 80), KeyActionType.MoveRight.Item2, 20, null, TextPosition.Start),
                        new MenuTextBox(new Vector2f(100, 510), new Vector2f(300, 80), KeyActionType.Shoot.Item2, 20, null, TextPosition.Start),
                        new MenuTextBox(new Vector2f(100, 610), new Vector2f(300, 80), KeyActionType.Pause.Item2, 20, null, TextPosition.Start),
                        new ChangeKeyButton(new Vector2f(600, 110), new Vector2f(300, 80), KeyActionType.MoveUp, 20, TextPosition.Start),
                        new ChangeKeyButton(new Vector2f(600, 210), new Vector2f(300, 80), KeyActionType.MoveDown, 20, TextPosition.Start),
                        new ChangeKeyButton(new Vector2f(600, 310), new Vector2f(300, 80), KeyActionType.MoveLeft, 20, TextPosition.Start),
                        new ChangeKeyButton(new Vector2f(600, 410), new Vector2f(300, 80), KeyActionType.MoveRight, 20, TextPosition.Start),
                        new ChangeKeyButton(new Vector2f(600, 510), new Vector2f(300, 80), KeyActionType.Shoot, 20, TextPosition.Start),
                        new ChangeKeyButton(new Vector2f(600, 610), new Vector2f(300, 80), KeyActionType.Pause, 20, TextPosition.Start),
                        BackButton
                    }
                },
                {
                    MenuLayer.EndScreen,
                    new List<Actor>
                    {
                        new MenuTextBox(new Vector2f(0, 110), new Vector2f(1000, 80), "GAME OVER", 40),
                        new MenuTextBox(new Vector2f(0, 310), new Vector2f(1000, 40), "Your Score:", 20),
                        ScoreBox,
                        PlayerNameInput,
                        new ActionTextButton(new Vector2f(0, 710), new Vector2f(1000, 80), "Return to menu", 30, OnBackToMenuClicked)
                    }
                },
                {
                    MenuLayer.Scores,
                    new List<Actor>
                    {
                        ScoresText,
                        BackButton,
                        ScoresPrev,
                        ScoresNext
                    }
                }
            };

            ShowMenu();
        }

        public void ShowMenu() => ShowLayer(MenuLayer.Main);
        public void ShowEndScreen() => ShowLayer(MenuLayer.EndScreen);
        public void Hide()
        {
            MenuBackground.Visible = false;
            Layers.Values.ToList().ForEach(layer 
                => layer.ForEach(renderable 
                    => renderable.Visible = false));
        }

        private void ShowLayer(MenuLayer layer)
        {
            MenuBackground.Visible = true;

            Refresh(1, layer);

            Layers.Values.ToList().ForEach(layer 
                => layer.ForEach(renderable 
                    => renderable.Visible = false));

            Refresh(2, layer);

            Layers[layer].ForEach(renderable 
                => renderable.Visible = true);

            Refresh(3, layer);
        }

        private void Refresh(int phase, MenuLayer layer)
        {
            switch (phase)
            {
                case 1:
                    ScoreBox.SetText(GamestateManager.Instance.Points.ToString());
                    break;
                case 2:
                    Layers[MenuLayer.Scores].Remove(ScoresText);
                    Layers[MenuLayer.Scores].Remove(BackButton);
                    Layers[MenuLayer.Scores].Remove(ScoresPrev);
                    Layers[MenuLayer.Scores].Remove(ScoresNext);

                    Layers[MenuLayer.Scores].ForEach(actor => actor.Dispose());
                    Layers[MenuLayer.Scores].Clear();

                    Layers[MenuLayer.Scores].Add(ScoresText);
                    Layers[MenuLayer.Scores].Add(BackButton);
                    Layers[MenuLayer.Scores].Add(ScoresPrev);
                    Layers[MenuLayer.Scores].Add(ScoresNext);

                    int i = 0;

                    ScoreManager.GetScores(10, ScoresOffset).ForEach(score =>
                    {
                        Layers[MenuLayer.Scores].AddRange(new List<Actor>
                        {
                            new MenuTextBox(new Vector2f(100, 210 + i * 50), new Vector2f(300, 50), score.Item1, 20),
                            new MenuTextBox(new Vector2f(400, 210 + i * 50), new Vector2f(300, 50), score.Item2, 20)
                        });
                        i++;
                    });
                    if (layer != MenuLayer.Scores) Layers[MenuLayer.Scores].ForEach(actor => actor.Visible = false);
                    break;
                case 3:
                    QuitNoSaveButton.Visible = layer == MenuLayer.Main && GamestateManager.Instance.GamePhase != GamePhase.NotStarted && GamestateManager.Instance.GamePhase != GamePhase.Ending;
                    ScoresPrev.Visible = layer == MenuLayer.Scores && ScoresOffset > 0;
                    ScoresNext.Visible = layer == MenuLayer.Scores && ScoresOffset + 10 < ScoreManager.GetScoresCount();
                    break;
            }
        }

        private void OnStartGameClicked(MouseButtonEventArgs mouseButtonEventArgs)    => MessageBus.Instance.PostEvent(MessageType.StartGame, this, new StartGameEventArgs(true));
        private void OnContinueGameClicked(MouseButtonEventArgs mouseButtonEventArgs) => MessageBus.Instance.PostEvent(MessageType.StartGame, this, new StartGameEventArgs(false));
        private void OnQuitClicked(MouseButtonEventArgs mouseButtonEventArgs)         => MessageBus.Instance.PostEvent(MessageType.Quit, this, new EventArgs());
        private void OnBackToMenuClicked(MouseButtonEventArgs mouseButtonEventArgs)   => MessageBus.Instance.PostEvent(MessageType.StopGame, this, new EventArgs());
        private void OnBackClicked(MouseButtonEventArgs mouseButtonEventArgs) => ShowLayer(MenuLayer.Main);
        private void OnKeysClicked(MouseButtonEventArgs mouseButtonEventArgs) => ShowLayer(MenuLayer.Keys);
        private void OnScoresClicked(MouseButtonEventArgs mouseButtonEventArgs) => ShowLayer(MenuLayer.Scores);

        private void OnQuitNoSaveButton(MouseButtonEventArgs obj)
        {
            if (GamestateManager.Instance.Player != null) GamestateManager.Instance.Player.OnDestroy();
        }

        private void OnNextClicked(MouseButtonEventArgs obj)
        {
            ScoresOffset += 10;
            OnScoresClicked(obj);
        }

        private void OnPrevClicked(MouseButtonEventArgs obj)
        {
            ScoresOffset -= 10;
            OnScoresClicked(obj);
        }

        public void Dispose()
        {
            MenuBackground.Dispose();
        }
    }
}
