using System;

namespace TankGame.Src.Data
{
    internal static class KeyActionType
    {
        public static Tuple<string, string> MoveUp => new Tuple<string, string>("keymoveup", "Move up");
        public static Tuple<string, string> MoveDown => new Tuple<string, string>("keymovedown", "Move down");
        public static Tuple<string, string> MoveLeft => new Tuple<string, string>("keymoveleft", "Move left");
        public static Tuple<string, string> MoveRight => new Tuple<string, string>("keymoveright", "Move right");
        public static Tuple<string, string> Shoot => new Tuple<string, string>("keyshoot", "Fire");
        public static Tuple<string, string> Pause => new Tuple<string, string>("keypause", "Pause");
    }
}