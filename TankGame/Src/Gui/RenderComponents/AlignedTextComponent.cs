using SFML.Graphics;
using SFML.System;
using System;
using TankGame.Src.Actors;

namespace TankGame.Src.Gui.RenderComponents
{
    public enum TextPosition
    {
        Start,
        Middle,
        End
    }

    class AlignedTextComponent : IRenderComponent
    {
        private Vector2f ContainerPosition { get; set; }
        private Vector2f ContainerSize { get; set; }
        private Vector2f Margins { get; }

        private readonly TextPosition HorizontalPosition;
        private readonly TextPosition VerticalPosition;

        private IRenderable Actor { get; }
        private readonly Font Font;

        private Text TextElement { get; }

        public AlignedTextComponent(Vector2f containerPosition, Vector2f containerSize, Vector2f margins, uint fontSize, TextPosition horizontalPosition, TextPosition verticalPosition, IRenderable actor, string text, Color color)
        {
            ContainerPosition = containerPosition;
            ContainerSize = containerSize;

            Margins = margins;

            HorizontalPosition = horizontalPosition;
            VerticalPosition = verticalPosition;

            Actor = actor;

            Font = new Font("Resources/Fonts/PressStart2P-Regular.ttf");

            TextElement = new Text(text, Font, fontSize)
            {
                FillColor = color
            };

            TextElement.Position = CalculatePosition();
        }

        public void SetFontSize(uint fontSize)
        {
            TextElement.CharacterSize = fontSize > 1 ? fontSize : 1;
            CalculatePosition();
        }

        public IRenderable GetActor()
        {
            return Actor;
        }

        public Drawable GetShape()
        {
            return TextElement;
        }

        public void SetSize(Vector2f size)
        {
            ContainerSize = size;
            CalculatePosition();
        }

        public void SetPosition(Vector2f position)
        {
            ContainerPosition = position;
            CalculatePosition();
        }

        public void SetColor(Color color)
        {
            TextElement.FillColor = color;
        }

        public void SetText(string text)
        {
            TextElement.DisplayedString = text;
            CalculatePosition();
        }

        public bool IsPointInside(Vector2f point)
        {
            return false;
        }

        private Vector2f CalculatePosition()
        {
            float x = 0;
            float y = 0;

            switch (HorizontalPosition)
            {
                case TextPosition.Start:
                    x = ContainerPosition.X + Margins.X;
                    break;

                case TextPosition.Middle:
                    x = ContainerPosition.X + ContainerSize.X / 2 - TextElement.GetGlobalBounds().Width / 2;
                    break;

                case TextPosition.End:
                    x = ContainerPosition.X + ContainerSize.X - TextElement.GetGlobalBounds().Width - Margins.X;
                    break;
            }

            switch (VerticalPosition)
            {
                case TextPosition.Start:
                    y = ContainerPosition.Y + Margins.Y;
                    break;

                case TextPosition.Middle:
                    y = ContainerPosition.Y + ContainerSize.Y / 2 - TextElement.CharacterSize / 1.5F;
                    break;

                case TextPosition.End:
                    y = ContainerPosition.Y + ContainerSize.Y - TextElement.GetGlobalBounds().Height - Margins.Y;
                    break;
            }

            return new Vector2f((float)Math.Ceiling(x), (float)Math.Floor(y));
        }
    }
}
