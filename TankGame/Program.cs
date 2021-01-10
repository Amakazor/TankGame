using TankGame.Src;

namespace TankGame
{
    internal class Program
    {
        private static void Main()
        {
            Engine engine = new Engine();
            engine.Loop();
        }
    }
}