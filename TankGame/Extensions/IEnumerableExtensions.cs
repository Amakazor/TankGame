using System;
using System.Collections.Generic;
using System.Linq;

namespace TankGame.Extensions; 

public static class IEnumerableExtensions {
   public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> source, int count = 1)
      => TakeRandom(source, new Random(), count);
   
   public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> source, Random randomnessSource, int count = 1)
      => source.OrderBy(x => randomnessSource.Next()).Take(count);
   
   public static IEnumerable<T> OrderRandomly<T>(this IEnumerable<T> source)
      => OrderRandomly(source, new Random());
   
   public static IEnumerable<T> OrderRandomly<T>(this IEnumerable<T> source, Random randomnessSource)
      => source.OrderBy(x => randomnessSource.Next());
}