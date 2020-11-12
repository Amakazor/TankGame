namespace TankGame.Src.Actors.Fields
{
    internal struct FieldData
    {
        public float SpeedModifier { get; }
        public bool IsTraversible { get; }

        public FieldData(float speedModifier, bool isTraversible)
        {
            SpeedModifier = speedModifier;
            IsTraversible = isTraversible;
        }
    }
}