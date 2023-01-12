namespace TankGame;

internal abstract class Program {
    public static void Main(string[] args) {
        Engine engine = new();
        engine.Loop();
    }
}