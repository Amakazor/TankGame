using System;
using SFML.Graphics;

namespace TankGame.Actors;

public abstract class ComplexShader : ITickable, IDisposable {
    protected ComplexShader(Shader shader) {
        Shader = shader;
        (this as ITickable).RegisterTickable();
    }

    public Shader Shader { get; }

    public void Dispose() {
        GC.SuppressFinalize(this);
        (this as ITickable).UnregisterTickable();
    }

    public abstract void Tick(float deltaTime);
}