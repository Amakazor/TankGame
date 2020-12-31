using System.Collections.Generic;

namespace TankGame.Src.Actors.Fields
{
    internal static class FieldType
    {
        public static readonly TraversibilityData Empty = new TraversibilityData(1, true);
        public static readonly TraversibilityData Grass = new TraversibilityData(2, true);
        public static readonly TraversibilityData Road = new TraversibilityData(1, true);
        public static readonly TraversibilityData Sand = new TraversibilityData(3, true);
        public static readonly TraversibilityData Water = new TraversibilityData(0, false);

        public static readonly Dictionary<string, TraversibilityData> FieldTypes = new Dictionary<string, TraversibilityData>
        {
            { "empty", Empty },
            { "grass", Grass },
            { "road", Road },
            { "sand", Sand },
            { "water", Water }
        };
    }
}
