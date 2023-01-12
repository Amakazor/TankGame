using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Buttons;

public abstract class TextButton : Button {
    protected AlignedTextComponent ButtonText;

    protected TextButton(Vector2f position, Vector2f size, string text, uint fontSize, TextPosition horizontalPosition = TextPosition.Middle, TextPosition verticalPosition = TextPosition.Middle) : base(position, size)
        => ButtonText = new(Position, Size, new(0, 0), fontSize, horizontalPosition, verticalPosition, text, Color.White);

    public override HashSet<IRenderComponent> RenderComponents {
        get {
            var renderComponents = base.RenderComponents;
            renderComponents.Add(ButtonText);

            return renderComponents;
        }
    }
}