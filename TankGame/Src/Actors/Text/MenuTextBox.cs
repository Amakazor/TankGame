using SFML.Graphics;
using SFML.System;
using TankGame.Src.Gui.RenderComponents;
using TankGame.Src.Actors.Data;

namespace TankGame.Src.Actors.Text
{
    class MenuTextBox : TextBox
    {
        public MenuTextBox(Vector2f position, Vector2f size, string text, uint fontSize, Color? textColor = null, TextPosition horizontalPosition = TextPosition.Middle, TextPosition verticalPosition = TextPosition.Middle) : base(position, size, text, fontSize, textColor, horizontalPosition, verticalPosition)
        {
            RenderLayer = RenderLayer.MenuFront;
            RenderView = RenderView.Menu;
        }
    }
}
