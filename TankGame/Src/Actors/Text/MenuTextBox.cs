﻿using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Gui.RenderComponents;

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