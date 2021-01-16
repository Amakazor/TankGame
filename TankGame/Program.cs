using TankGame.Src;

namespace TankGame
{
    static class Program
    {
        private static void Main()
        {
            Engine engine = new Engine();
            engine.Loop();
        }
    }
}