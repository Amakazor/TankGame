using SFML.Window;

namespace TankGame.Src.Actors
{
    internal interface IClickable : IRenderable
    {
        public bool OnClick(MouseButtonEventArgs args);
        public void RegisterClickable(IClickable clickable);
        public void UnregisterClickable(IClickable clickable);
    }
}
