namespace TankGame.Src.Actors
{
    internal interface IDestructible
    {
        public bool IsAlive { get; }
        public int Health { get; set; }
        public bool IsDestructible { get; }
        public bool StopsProjectile { get; }
        public Actor Actor { get; }

        public void OnHit();
        public void OnDestroy();
        public void RegisterDestructible();
        public void UnregisterDestructible();
    }
}