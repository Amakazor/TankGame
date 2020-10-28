using System;
using TankGame.Src;

namespace TankGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine engine = new Engine();
            engine.Loop();
        }
    }
}
