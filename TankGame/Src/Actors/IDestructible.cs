namespace TankGame.Src.Actors
{
    internal interface IDestructible
    {
        public void OnHit(Actor other);
        public void OnDestroy(Actor other);
        public bool IsAlive();
        public int GetHelth();
        public void SetHealth(int amount);
    }
}