using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Text;

public class TextBox : Actor {
    protected AlignedTextComponent Text;

    public TextBox(Vector2f position, Vector2f size, string text, uint fontSize, Color? textColor = null, TextPosition horizontalPosition = TextPosition.Middle, TextPosition verticalPosition = TextPosition.Middle) : base(position, size)
        => Text = new(Position, Size, new(0, 0), fontSize, horizontalPosition, verticalPosition, text, textColor ?? Color.White);

    public override HashSet<IRenderComponent> RenderComponents => new() { Text };

    public void SetText(string newText)
        => Text.SetText(newText);
}