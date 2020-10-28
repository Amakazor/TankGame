namespace TankGame.Src.Actors
{
    internal interface ITickable
    {
        public void Tick(float deltaTime);
        public void RegisterTickable(ITickable tickable);
        public void UnregisterTickable(ITickable tickable);
    }
}
