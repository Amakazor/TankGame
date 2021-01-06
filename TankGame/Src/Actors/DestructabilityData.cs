namespace TankGame.Src.Actors
{
    internal struct DestructabilityData
    {
        public int Health { get; }
        public bool IsDestructible { get; }
        public bool DestroyOnEntry { get; }
        public bool StopsProjectile { get; }

        public DestructabilityData(int health, bool isDestructible, bool destroyOnEntry, bool stopsProjectile = true)
        {
            Health = health;
            IsDestructible = isDestructible;
            DestroyOnEntry = destroyOnEntry;
            StopsProjectile = stopsProjectile;
        }
    }
}