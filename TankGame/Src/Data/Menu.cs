using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TankGame.Src.Actors;
using TankGame.Src.Actors.Background;
using TankGame.Src.Actors.Buttons;
using TankGame.Src.Actors.Text;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Data
{
    class Menu : IDisposable
    {
        private MenuBackground MenuBackground;
        private Dictionary<MenuLayer, List<Actor>> Layers;

        private MenuTextBox MoveUpKey;
        private MenuTextBox MoveDownKey;
        private MenuTextBox MoveLeftKey;
        private MenuTextBox MoveRightKey;

        public Menu()
        {
            MenuBackground = new MenuBackground();

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
                        new ActionTextButton(new Vector2f(0, 510), new Vector2f(1000, 80), "Quit", 30, OnQuitClicked)
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
                        new ChangeKeyButton(new Vector2f(600, 110), new Vector2f(300, 80), KeyActionType.MoveUp, 20, TextPosition.Start),
                        new ChangeKeyButton(new Vector2f(600, 210), new Vector2f(300, 80), KeyActionType.MoveDown, 20, TextPosition.Start),
                        new ChangeKeyButton(new Vector2f(600, 310), new Vector2f(300, 80), KeyActionType.MoveLeft, 20, TextPosition.Start),
                        new ChangeKeyButton(new Vector2f(600, 410), new Vector2f(300, 80), KeyActionType.MoveRight, 20, TextPosition.Start),
                        new ChangeKeyButton(new Vector2f(600, 510), new Vector2f(300, 80), KeyActionType.Shoot, 20, TextPosition.Start),
                        new ActionTextButton(new Vector2f(0, 710), new Vector2f(1000, 80), "Back", 30, OnBackClicked)
                    }
                }
            };

            Show();
        }

        public void Show() => ShowLayer(MenuLayer.Main);
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

            Layers.Values.ToList().ForEach(layer 
                => layer.ForEach(renderable 
                    => renderable.Visible = false));

            Layers[layer].ForEach(renderable 
                => renderable.Visible = true);
        }

        private void OnStartGameClicked(MouseButtonEventArgs mouseButtonEventArgs)
        {
            MessageBus.Instance.PostEvent(MessageType.StartGame, this, new StartGameEventArgs(true));
        }
        
        private void OnContinueGameClicked(MouseButtonEventArgs mouseButtonEventArgs)
        {
            MessageBus.Instance.PostEvent(MessageType.StartGame, this, new StartGameEventArgs(false));
        }
        
        private void OnKeysClicked(MouseButtonEventArgs mouseButtonEventArgs)
        {
            ShowLayer(MenuLayer.Keys);
        }
        
        private void OnScoresClicked(MouseButtonEventArgs mouseButtonEventArgs)
        {
            
        }
        
        private void OnQuitClicked(MouseButtonEventArgs mouseButtonEventArgs)
        {
            MessageBus.Instance.PostEvent(MessageType.Quit, this, new EventArgs());
        }
        
        private void OnBackClicked(MouseButtonEventArgs mouseButtonEventArgs)
        {
            ShowLayer(MenuLayer.Main);
        }

        public void Dispose()
        {
            MenuBackground.Dispose();
        }
    }
}
