using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Buttons
{
    internal class TextInput : Button
    {
        public string Text { get; protected set; }

        protected AlignedTextComponent InputText { get; }
        protected AlignedTextComponent InputPlaceholderText { get; }
        private bool Focused { get; set; }

        public TextInput(Vector2f position, Vector2f size, string placeholderText, uint fontSize, TextPosition horizontalPosition = TextPosition.Middle, TextPosition verticalPosition = TextPosition.Middle) : base(position, size)
        {
            Focused = false;

            InputText = new AlignedTextComponent(position, size, new Vector2f(0, 0), fontSize, horizontalPosition, verticalPosition, this, "", new Color(255, 255, 255, 255));
            InputPlaceholderText = new AlignedTextComponent(position, size, new Vector2f(0, 0), fontSize, horizontalPosition, verticalPosition, this, placeholderText, new Color(200, 200, 200, 255));
            BoundingBox = new RectangleComponent(position, size, this, new Color(32, 32, 32, 255), new Color(128, 128, 128, 255), 2);

            MessageBus.Instance.Register(MessageType.CancelInputs, OnCancelInputs);
            MessageBus.Instance.Register(MessageType.TextInput, OnKeyPressed);
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { BoundingBox, Text == "" || Text == null ? InputPlaceholderText : null, InputText };
        }

        private void OnKeyPressed(object sender, EventArgs eventArgs)
        {
            if (Focused && eventArgs is TextEventArgs textEventArgs)
            {
                if (textEventArgs.Unicode == "\b" && Text.Length != 0) Text = Text.Remove(Text.Length - 1);
                else if (textEventArgs.Unicode != "\x1B" && textEventArgs.Unicode != "\t" && textEventArgs.Unicode != "\b" && (Text == null || Text.Length < 10)) Text += textEventArgs.Unicode;
            }

            if (Text != null) InputText.SetText(Text);
        }

        private void OnCancelInputs(object sender, EventArgs eventArgs)
        {
            Focused = false;
        }

        public override void Dispose()
        {
            MessageBus.Instance.Unregister(MessageType.CancelInputs, OnCancelInputs);
            MessageBus.Instance.Unregister(MessageType.TextInput, OnKeyPressed);
            base.Dispose();
        }

        public override bool OnClick(MouseButtonEventArgs eventArgs)
        {
            MessageBus.Instance.PostEvent(MessageType.CancelInputs, this, new EventArgs());
            return Focused = true;
        }
    }
}
