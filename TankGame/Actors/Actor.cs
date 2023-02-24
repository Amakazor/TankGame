using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors;

public abstract class Actor : IRenderable, IDisposable {
    protected Actor(Vector2f position, Vector2f size) {
        Position = position;
        Size = size;

        (this as IRenderable).RegisterRenderable();
        Visible = true;
    }
    
    public static Actor ToActor<T>(T actor) where T : Actor
        => actor;

    [JsonIgnore] public Vector2f Position { get; protected set; }
    [JsonIgnore] public Vector2f Size { get; }
    protected RenderLayer RenderLayer { get; set; }
    protected RenderView RenderView { get; set; }

    public virtual void Dispose() {
        GC.SuppressFinalize(this);
        (this as IRenderable).UnregisterRenderable();
    }
    
    [JsonIgnore] public RenderLayer RenderableRenderLayer => RenderLayer;
    [JsonIgnore] public RenderView RenderableRenderView => RenderView;
    [JsonIgnore] public bool Visible { get; set; }
    [JsonIgnore] public abstract HashSet<IRenderComponent> RenderComponents { get; }
}