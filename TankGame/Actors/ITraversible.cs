namespace TankGame.Actors; 

public interface ITraversible {
    public float SpeedModifier { get; }
    public bool Traversible { get; }
}