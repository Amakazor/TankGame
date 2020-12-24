using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Actors.Fields;

namespace TankGame.Src.Actors.GameObjects
{
    internal static class GameObjectType
    {
        public static readonly Tuple<TraversibilityData, DestructabilityData> Building = new Tuple<TraversibilityData, DestructabilityData> (new TraversibilityData(0F, false), new DestructabilityData(1, false, false));
        public static readonly Tuple<TraversibilityData, DestructabilityData> House = new Tuple<TraversibilityData, DestructabilityData> (new TraversibilityData(0F, false), new DestructabilityData(2, true, false));
        public static readonly Tuple<TraversibilityData, DestructabilityData> Fence1 = new Tuple<TraversibilityData, DestructabilityData> (new TraversibilityData(2F, true), new DestructabilityData(1, true, true));
        public static readonly Tuple<TraversibilityData, DestructabilityData> Fence2 = new Tuple<TraversibilityData, DestructabilityData> (new TraversibilityData(2F, true), new DestructabilityData(1, true, true));
        public static readonly Tuple<TraversibilityData, DestructabilityData> Tree = new Tuple<TraversibilityData, DestructabilityData> (new TraversibilityData(3F, true), new DestructabilityData(1, true, true));
        
        public static readonly Dictionary<string, Tuple<TraversibilityData, DestructabilityData>> GameObjectTypes = new Dictionary<string, Tuple<TraversibilityData, DestructabilityData>>
        {
            { "bigbuilding", Building },
            { "house", House },
            { "fence1", Fence1 },
            { "fence2", Fence2 },
            { "tree", Tree },
        };
    }
}
