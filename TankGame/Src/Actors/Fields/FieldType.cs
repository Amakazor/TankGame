using System.Collections.Generic;

namespace TankGame.Src.Actors.Fields
{
    internal static class FieldType
    {
        public static readonly FieldData Empty = new FieldData(1, true);
        public static readonly FieldData Grass = new FieldData(1.5F, true);
        public static readonly FieldData Road = new FieldData(1, true);
        public static readonly FieldData Sand = new FieldData(2, true);
        public static readonly FieldData Water = new FieldData(0, false);

        public static readonly Dictionary<string, FieldData> FieldTypes = new Dictionary<string, FieldData>
        {
            { "empty", Empty },
            { "grass", Grass },
            { "road", Road },
            { "sand", Sand },
            { "water", Water }
        };
    }
}
