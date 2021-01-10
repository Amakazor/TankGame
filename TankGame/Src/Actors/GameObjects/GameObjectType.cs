using System;
using System.Collections.Generic;
using TankGame.Src.Actors.Data;

namespace TankGame.Src.Actors.GameObjects
{
    internal static class GameObjectType
    {
        public static readonly Tuple<TraversibilityData, DestructabilityData, string> Building  = new Tuple<TraversibilityData, DestructabilityData, string> (new TraversibilityData(0F, false),   new DestructabilityData(1, false, false),        null);
        public static readonly Tuple<TraversibilityData, DestructabilityData, string> House     = new Tuple<TraversibilityData, DestructabilityData, string> (new TraversibilityData(0F, false),   new DestructabilityData(4, true, false),         "gruz");
        public static readonly Tuple<TraversibilityData, DestructabilityData, string> Fence1    = new Tuple<TraversibilityData, DestructabilityData, string> (new TraversibilityData(1.5F, true),  new DestructabilityData(2, true, true),          "gruz");
        public static readonly Tuple<TraversibilityData, DestructabilityData, string> Fence2    = new Tuple<TraversibilityData, DestructabilityData, string> (new TraversibilityData(1.5F, true),  new DestructabilityData(2, true, true),          "gruz");
        public static readonly Tuple<TraversibilityData, DestructabilityData, string> Gruz      = new Tuple<TraversibilityData, DestructabilityData, string> (new TraversibilityData(1.25F, true), new DestructabilityData(1, false, false, false), null);
        public static readonly Tuple<TraversibilityData, DestructabilityData, string> Tree      = new Tuple<TraversibilityData, DestructabilityData, string> (new TraversibilityData(2F, true),    new DestructabilityData(1, true, true),          "pien");
        public static readonly Tuple<TraversibilityData, DestructabilityData, string> Tree2     = new Tuple<TraversibilityData, DestructabilityData, string> (new TraversibilityData(2F, true),    new DestructabilityData(1, true, true),          "pien");
        public static readonly Tuple<TraversibilityData, DestructabilityData, string> Pien      = new Tuple<TraversibilityData, DestructabilityData, string> (new TraversibilityData(1.25F, true), new DestructabilityData(1, false, false, false), null);
        
        public static readonly Dictionary<string, Tuple<TraversibilityData, DestructabilityData, string>> GameObjectTypes = new Dictionary<string, Tuple<TraversibilityData, DestructabilityData, string>>
        {
            { "bigbuilding", Building },
            { "house", House },
            { "fence1", Fence1 },
            { "fence2", Fence2 },
            { "gruz", Gruz },
            { "tree", Tree },
            { "tree2", Tree2 },
            { "pien", Pien },
        };
    }
}
