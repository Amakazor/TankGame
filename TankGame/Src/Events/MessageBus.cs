using System;
using System.Collections.Generic;

namespace TankGame.Src.Events
{
    class MessageBus
    {
        private static MessageBus instance;
        private Dictionary<MessageType, List<Action<object, EventArgs>>> Listeners = new Dictionary<MessageType, List<Action<object, EventArgs>>>();
        private List<Tuple<MessageType, Action<object, EventArgs>>> ToUnregister = new List<Tuple<MessageType, Action<object, EventArgs>>>();

        private MessageBus()
        {
        }

        public static MessageBus Instance { get { return instance ?? (instance = new MessageBus()); } }

        public void Register(MessageType messageType, Action<object, EventArgs> listener)
        {
            if (!Listeners.ContainsKey(messageType))
            {
                Listeners[messageType] = new List<Action<object, EventArgs>>();
            }

            Listeners[messageType].Add(listener);
        }

        public void Unregister(MessageType messageType, Action<object, EventArgs> listener)
        {
            ToUnregister.Add(new Tuple<MessageType, Action<object, EventArgs>>(messageType, listener));
        }

        private void FinalizeUnregistering()
        {
            foreach (Tuple<MessageType, Action<object, EventArgs>> dataToUnregister in ToUnregister)
            {
                if (Listeners.ContainsKey(dataToUnregister.Item1) && Listeners[dataToUnregister.Item1].Contains(dataToUnregister.Item2))
                {
                    Listeners[dataToUnregister.Item1].Remove(dataToUnregister.Item2);
                }
                if (Listeners.ContainsKey(dataToUnregister.Item1) && Listeners[dataToUnregister.Item1].Contains(dataToUnregister.Item2))
                {
                    Listeners[dataToUnregister.Item1].Remove(dataToUnregister.Item2);
                }
            }

            ToUnregister.Clear();
        }

        public void PostEvent(MessageType messageType, object sender, EventArgs eventArgs)
        {
            FinalizeUnregistering();

            if (Listeners.ContainsKey(messageType))
            {
                Listeners[messageType].ForEach((Action<object, EventArgs> listener) =>
                {
                    listener.Invoke(sender, eventArgs);
                });
            }
        }
    }
}
