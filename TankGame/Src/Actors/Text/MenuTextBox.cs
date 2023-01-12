using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Data;
using TankGame.Gui.RenderComponents;

namespace TankGame.Actors.Text;

public class MenuTextBox : TextBox {
    public MenuTextBox(Vector2f position, Vector2f size, string text, uint fontSize, Color? textColor = null, TextPosition horizontalPosition = TextPosition.Middle, TextPosition verticalPosition = TextPosition.Middle) : base(position, size, text, fontSize, textColor, horizontalPosition, verticalPosition) {
        RenderLayer = RenderLayer.MenuFront;
        RenderView = RenderView.Menu;
    }
}