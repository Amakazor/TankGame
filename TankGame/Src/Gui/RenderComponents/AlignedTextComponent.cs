using SFML.Graphics;
using SFML.System;
using System;

namespace TankGame.Src.Gui.RenderComponents
{
    public enum TextPosition
    {
        Start,
        Middle,
        End
    }

    internal class AlignedTextComponent : IRenderComponent
    {
        private Vector2f ContainerPosition { get; set; }
        private Vector2f ContainerSize { get; set; }
        private Vector2f Margins { get; }

        private TextPosition HorizontalPosition { get; }
        private TextPosition VerticalPosition { get; }

        private Text TextElement { get; }

        public bool IsPointInside(Vector2f point) => false;

        public AlignedTextComponent(Vector2f containerPosition, Vector2f containerSize, Vector2f margins, uint fontSize, TextPosition horizontalPosition, TextPosition verticalPosition, string text, Color color)
        {
            ContainerPosition = containerPosition;
            ContainerSize = containerSize;

            Margins = margins;

            HorizontalPosition = horizontalPosition;
            VerticalPosition = verticalPosition;

            TextElement = new Text(text, new Font("Resources/Fonts/PressStart2P-Regular.ttf"), fontSize)
            {
                FillColor = color,
            };

            TextElement.Position = CalculatePosition();
        }

        public void SetFontSize(uint fontSize)
        {
            TextElement.CharacterSize = fontSize > 1 ? fontSize : 1;
            TextElement.Position = CalculatePosition();
        }

        public Drawable Shape => TextElement;

        public void SetSize(Vector2f size)
        {
            ContainerSize = size;
            TextElement.Position = CalculatePosition();
        }

        public void SetPosition(Vector2f position)
        {
            ContainerPosition = position;
            TextElement.Position = CalculatePosition();
        }

        public void SetColor(Color color) => TextElement.FillColor = color;

        public void SetText(string text)
        {
            TextElement.DisplayedString = text;
            TextElement.Position = CalculatePosition();
        }

        private Vector2f CalculatePosition()
        {
            float x = HorizontalPosition switch
            {
                TextPosition.Start  => ContainerPosition.X + Margins.X,
                TextPosition.Middle => ContainerPosition.X + ContainerSize.X / 2 - TextElement.GetGlobalBounds().Width / 2,
                TextPosition.End    => ContainerPosition.X + ContainerSize.X - TextElement.GetGlobalBounds().Width - Margins.X,
                _ => throw new NotImplementedException(),
            };
            
            float y = VerticalPosition switch
            {
                TextPosition.Start  => ContainerPosition.Y + Margins.Y,
                TextPosition.Middle => ContainerPosition.Y + ContainerSize.Y / 2 - TextElement.CharacterSize / 1.5F,
                TextPosition.End    => ContainerPosition.Y + ContainerSize.Y - TextElement.GetGlobalBounds().Height - Margins.Y,
                _ => throw new NotImplementedException(),
            };
            
            return new Vector2f((float)Math.Ceiling(x), (float)Math.Floor(y));
        }
    }
}