namespace TankGame.Src.Actors
{
    internal interface IDestructible
    {
        public void OnHit(Actor other);
        public void OnDestroy(Actor other);
        public bool IsAlive();
        public int GetHealth();
        public void SetHealth(int amount);
        bool IsDestructible();
    }
}