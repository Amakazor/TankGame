using System;
using System.Collections.Generic;
using System.Text;

namespace TankGame.Src
{
    internal static class Utility
    {
        private static readonly Random random = new Random();

        public static int GetRandomInt(int min, int max) 
        {
            return random.Next(min, max);
        }
    }
}
