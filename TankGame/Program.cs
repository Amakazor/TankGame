using System;
using TankGame.Src;

namespace TankGame
{
    static class Program
    {
        private static void Main()
        {
            try
            {
                Engine engine = new Engine();
                engine.Loop();
            }
            catch (Exception e)
            {
                #if DEBUG
                    throw;
                #else
                    throw new InvalidOperationException("Application cannot be run, please reinstall application", e);
                #endif
            }
        }
    }
}