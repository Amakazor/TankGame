using SFML.Window;

namespace TankGame.Src.Actors
{
    internal interface IClickable : IRenderable
    {
        public bool OnClick(MouseButtonEventArgs eventArgs);

        public void RegisterClickable();

        public void UnregisterClickable();
    }
}