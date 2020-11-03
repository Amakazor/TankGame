namespace TankGame.Src.Actors
{
    internal interface ITickable
    {
        public void Tick(float deltaTime);
        public void RegisterTickable();
        public void UnregisterTickable();
    }
}
