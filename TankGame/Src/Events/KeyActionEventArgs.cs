using System;

namespace TankGame.Src.Events
{
    class KeyActionEventArgs : EventArgs
    {
        public Tuple<string, string> KeyActionType;

        public KeyActionEventArgs(Tuple<string, string> keyActionType)
        {
            KeyActionType = keyActionType;
        }
    }
}
