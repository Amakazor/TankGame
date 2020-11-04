using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Buttons
{
    internal abstract class TextButton : Button
    {
        protected AlignedTextComponent ButtonText;

        public TextButton(Vector2f position, Vector2f size, string text, uint fontSize) : base(position, size)
        {
            ButtonText = new AlignedTextComponent(Position, Size, new Vector2f(0, 0), fontSize, TextPosition.Middle, TextPosition.Middle, this, text, Color.White);
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            HashSet<IRenderComponent> renderComponents = base.GetRenderComponents();
            renderComponents.Add(ButtonText);

            return renderComponents;
        }
    }
}