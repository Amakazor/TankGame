using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SFML.Window;

namespace TankGame.Core.Controls;

public static class KeyManager {
    static KeyManager()
        => KeyActions = LoadKeys();

    private static HashSet<KeyAction> KeyActions { get; set; }

    public static void ChangeAndSaveKey(KeyAction keyAction) {
        if (!KeyActions.Contains(keyAction)) return;

        KeyActions.Remove(keyAction);
        KeyActions.Add(keyAction);

        SaveKeys();
        KeyActions = LoadKeys();
    }

    public static Keyboard.Key GetKey(Action action)
        => KeyActions.FirstOrDefault(keyAction => keyAction.Action == action)
                    ?.Key ?? Keyboard.Key.Unknown;

    public static Action GetAction(Keyboard.Key key)
        => KeyActions.FirstOrDefault(keyAction => keyAction.Key == key)
                    ?.Action ?? Action.Nothing;

    private static HashSet<KeyAction> LoadKeys() {
        string json = File.ReadAllText("Resources/Config/Keys.json");
        return JsonSerializer.Deserialize<HashSet<KeyAction>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new HashSet<KeyAction>();
    }

    private static void SaveKeys() {
        string json = JsonSerializer.Serialize(KeyActions, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        File.WriteAllText("Resources/Config/Keys.json", json);
    }
}