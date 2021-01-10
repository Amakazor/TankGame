using System.Collections.Generic;
using System.Linq;

namespace TankGame.Src.Extensions
{
    internal static class HashSetExtensions
    {
        public static HashSet<T> AddDeleteRange<T>(this HashSet<T> value, HashSet<T> add, HashSet<T> del)
        {
            add.ToList().ForEach(addition => value.Add(addition));
            del.ToList().ForEach(deletion => value.Remove(deletion));
            return value;
        }
    }
}
