namespace TankGame.Src.Actors
{
    internal interface IDestructible
    {
        public void OnHit(Actor other);
        public void OnDestroy(Actor other);
        public bool IsAlive { get; }
        public int Health { get; set; }
        bool IsDestructible { get; }
    }
}