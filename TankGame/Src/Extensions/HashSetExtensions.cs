using System.Collections.Generic;
using System.Linq;

namespace TankGame.Extensions;

public static class HashSetExtensions {
    public static void AddDeleteRange<T>(this HashSet<T> value, IEnumerable<T> add, IEnumerable<T> del) {
        value.AddRange(add);
        value.RemoveRange(del);
    }

    public static void AddRange<T>(this HashSet<T> value, IEnumerable<T> add)
        => add.ToList()
              .ForEach(addition => value.Add(addition));

    public static void RemoveRange<T>(this HashSet<T> value, IEnumerable<T> del)
        => del.ToList()
              .ForEach(deletion => value.Remove(deletion));
}