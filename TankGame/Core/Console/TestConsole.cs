using System;
using System.Linq;
using System.Text;
using SFML.Window;
using TankGame.Actors.Inputs;
using TankGame.Core.Console.Commands;
using TankGame.Core.Controls;
using TankGame.Events;

namespace TankGame.Core.Console; 

public class TestConsole : IDisposable {
    public TestConsole() {
        MessageBus.KeyAction += OnKeyAction;
        MessageBus.TextInput += OnTextInput;
    }

    private ConsoleInput ConsoleInput { get; } = new(new(0, 0), new(1000, 40));
    private StringBuilder Text { get; set; } = new();
    
    private void OnKeyAction(InputAction inputAction) {
        if (inputAction == InputAction.ToggleConsole) ConsoleInput.Visible = !ConsoleInput.Visible;
    }

    private void OnTextInput(TextEventArgs textArgs) {
        if (!ConsoleInput.Visible) return;

        if (textArgs.Unicode == "\r" && Text.Length != 0) {
            ParseText(Text.ToString());
            Text.Clear();
        }
        
        if (textArgs.Unicode == "\b" && Text.Length != 0) {
            Text = Text.Remove(Text.Length - 1, 1);
        }

        if (textArgs.Unicode != "\x1B" && textArgs.Unicode != "\t" && textArgs.Unicode != "\b" && textArgs.Unicode != "\r") {
            Text.Append(textArgs.Unicode);
        }

        ConsoleInput.InputText.SetText(Text.ToString());
    }

    public void Dispose() {
        MessageBus.KeyAction -= OnKeyAction;
        MessageBus.TextInput -= OnTextInput;
    }

    private static void ParseText(string text)
        => CommandFactory
          .CreateCommand(Seq(text.Split(' ').Select(part => part.Trim())))
          .IfSome(cmd => cmd.Execute());
}