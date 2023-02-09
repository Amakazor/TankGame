using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Core.Fonts;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Inputs; 

public class ConsoleInput : Actor {
    private readonly HashSet<IRenderComponent> _renderComponents;

    public ConsoleInput(Vector2f position, Vector2f size) : base(position, size) {
        BoundingBox = new(position, size, Color.White, Color.Black, 2);
        InputText = new(position, size, new(20, 0), 15, TextPosition.Start, TextPosition.Middle, "CONSOLETEST" , Color.Black, FontFile.Iosevka);
        
        _renderComponents = new HashSet<IRenderComponent> {
            BoundingBox,
            InputText,
        };
        
        RenderLayer = RenderLayer.Console;
        RenderView = RenderView.Console;
    }
    
    public RectangleComponent BoundingBox { get; }
    public AlignedTextComponent InputText { get; }
    public override HashSet<IRenderComponent> RenderComponents => _renderComponents;
}