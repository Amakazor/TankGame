namespace TankGame.Src.Actors
{
    internal interface IDestructible
    {
        public void OnHit();
        public void OnDestroy();
        public bool IsAlive { get; }
        public int Health { get; set; }
        public bool IsDestructible { get; }
        public bool StopsProjectile { get; }
        public Actor Actor { get; }
        public void RegisterDestructible();
        public void UnregisterDestructible();
    }
}