using System;
using System.Collections.Generic;
using System.Linq;

namespace TankGame.Src.Extensions
{
    internal static class HashSetExtensions
    {
        public static HashSet<T> AddDeleteRange<T>(this HashSet<T> value, HashSet<T> add, HashSet<T> delete)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (add is null) throw new ArgumentNullException(nameof(add));
            if (delete is null) throw new ArgumentNullException(nameof(delete));

            add.ToList().ForEach(addition => value.Add(addition));
            delete.ToList().ForEach(deletion => value.Remove(deletion));

            return value;
        }
    }
}
