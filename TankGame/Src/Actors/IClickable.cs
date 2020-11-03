using SFML.Window;

namespace TankGame.Src.Actors
{
    internal interface IClickable
    {
        public bool OnClick(MouseButtonEventArgs args);
        public void RegisterClickable(IClickable clickable);
        public void UnregisterClickable(IClickable clickable);
    }
}
