using System.Collections.Generic;
using SFML.Graphics;
using TankGame.Actors.Data;
using TankGame.Events;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors;

public interface IRenderable {
    public bool Visible { get; set; }
    public RenderLayer RenderableRenderLayer { get; }
    public RenderView RenderableRenderView { get; }

    HashSet<IRenderComponent> RenderComponents { get; }

    public Shader? CurrentShader => null;

    public void RegisterRenderable()
        => MessageBus.RegisterRenderable.Invoke(this);

    public void UnregisterRenderable()
        => MessageBus.UnregisterRenderable.Invoke(this);
}