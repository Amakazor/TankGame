namespace TankGame.Src.Actors.Data
{
    internal struct TraversibilityData
    {
        public float SpeedModifier { get; }
        public bool IsTraversible { get; }

        public TraversibilityData(float speedModifier, bool isTraversible)
        {
            SpeedModifier = speedModifier;
            IsTraversible = isTraversible;
        }
    }
}