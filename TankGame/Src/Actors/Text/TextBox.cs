using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Text
{
    internal class TextBox : Actor
    {
        protected AlignedTextComponent Text;

        public TextBox(Vector2f position, Vector2f size, string text, uint fontSize, Color? textColor = null, TextPosition horizontalPosition = TextPosition.Middle, TextPosition verticalPosition = TextPosition.Middle) : base(position, size)
        {
            Text = new AlignedTextComponent(Position, Size, new Vector2f(0, 0), fontSize, horizontalPosition, verticalPosition, this, text, textColor ?? Color.White);
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { Text };
        }

        public void SetText(string newText) => Text.SetText(newText);
    }
}
