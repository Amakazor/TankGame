namespace TankGame.Src.Actors
{
    internal struct DestructabilityData
    {
        public int HP { get; }
        public bool IsDestructible { get; }
        public bool DestroyOnEntry { get; }

        public DestructabilityData(int hp, bool isDestructible, bool destroyOnEntry)
        {
            HP = hp;
            IsDestructible = isDestructible;
            DestroyOnEntry = destroyOnEntry;
        }
    }
}