using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace TankGame.Src.Data
{
    internal class KeyManager
    {
        private static KeyManager instance;

        private Dictionary<Tuple<string, string>, Keyboard.Key> KeysDictionary;

        private KeyManager()
        {
            LoadKeys();
        }

        public static KeyManager Instance { get { return instance ?? (instance = new KeyManager()); } }

        public static void Initialize() => _ = Instance;

        public void ChangeAndSaveKey(Tuple<string, string> keyActionType, Keyboard.Key keyCode)
        {
            if (KeysDictionary.ContainsKey(keyActionType) && KeysDictionary[keyActionType] != keyCode)
            {
                KeysDictionary[keyActionType] = keyCode;
                SaveKeys();
                LoadKeys();
            }
            else throw new ArgumentException("There is no key for this action", "keyActionType");
        }

        public Keyboard.Key GetKey(Tuple<string, string> keyActionType)
        {
            if (KeysDictionary.ContainsKey(keyActionType))
            {
                return KeysDictionary[keyActionType];
            }
            else throw new ArgumentException("There is no key for this action", "keyActionType");
        }

        public Tuple<string, string> GetAction(Keyboard.Key key)
        {
            foreach (KeyValuePair<Tuple<string, string>, Keyboard.Key> keyActionKeyPair in KeysDictionary)
            {
                if (keyActionKeyPair.Value == key)
                {
                    return keyActionKeyPair.Key;
                }
            }

            return null;
        }

        private void LoadKeys()
        {
            KeysDictionary = new Dictionary<Tuple<string, string>, Keyboard.Key>();

            XDocument keysConfig;

            try
            {
                keysConfig = XDocument.Load("Resources/Config/Keys.xml");
            }
            catch (Exception)
            {
                throw new Exception("File Keys.xml couldn't be loaded");
            }

            foreach (PropertyInfo propertyInfo in typeof(KeyActionType).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                try
                {
                    if (propertyInfo.GetValue(null) is Tuple<string, string> keyActionType)
                    {
                        KeysDictionary[keyActionType] = (Keyboard.Key)int.Parse(keysConfig.Descendants("keys").Select(x => x.Element(keyActionType.Item1).Value).FirstOrDefault());
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Keys file couldn't be loaded");
                }
            }
        }

        private void SaveKeys()
        {
            XDocument keysConfig;

            try
            {
                keysConfig = XDocument.Load("Resources/Config/Keys.xml");
            }
            catch (Exception)
            {
                throw new Exception("File Keys.xml couldn't be loaded");
            }

            foreach (KeyValuePair<Tuple<string, string>, Keyboard.Key> key in KeysDictionary)
            {
                if (keysConfig.Element(key.Key.Item1) != null)
                {
                    XElement keyElement = keysConfig.Element(key.Key.Item1);
                    keyElement.Value = ((int)key.Value).ToString();
                }
            }

            keysConfig.Save("Resources/Config/Keys.xml");
        }
    }
}