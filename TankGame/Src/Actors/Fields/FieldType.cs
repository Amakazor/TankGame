using System;
using System.Collections.Generic;
using TankGame.Src.Actors.Data;

namespace TankGame.Src.Actors.Fields
{
    internal static class FieldType
    {
        public static readonly Tuple<TraversibilityData, bool> Empty = new Tuple<TraversibilityData, bool>(new TraversibilityData(1, true),     false);
        public static readonly Tuple<TraversibilityData, bool> Grass = new Tuple<TraversibilityData, bool>(new TraversibilityData(1.33F, true), true);
        public static readonly Tuple<TraversibilityData, bool> Road  = new Tuple<TraversibilityData, bool>(new TraversibilityData(1, true),     false);
        public static readonly Tuple<TraversibilityData, bool> Sand  = new Tuple<TraversibilityData, bool>(new TraversibilityData(2F, true),    true);
        public static readonly Tuple<TraversibilityData, bool> Water = new Tuple<TraversibilityData, bool>(new TraversibilityData(0, false),    true);

        public static readonly Dictionary<string, Tuple<TraversibilityData, bool>> FieldTypes = new Dictionary<string, Tuple<TraversibilityData, bool>>
        {
            { "empty", Empty },
            { "grass", Grass },
            { "road", Road },
            { "sand", Sand },
            { "water", Water }
        };
    }
}
