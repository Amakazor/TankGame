using SFML.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace TankGame.Src.Data
{
    internal class SoundManager
    {
        private static SoundManager instance;

        private Dictionary<string, Dictionary<string, SoundBuffer>> SoundsDictionary { get; set; }

        private SoundManager()
        {
            LoadSound();
        }

        public static SoundManager Instance { get { return instance ?? (instance = new SoundManager()); } }

        public static void Initialize()
        {
            _ = Instance;
        }

        public SoundBuffer GetSound(string soundType, string name)
        {
            if (SoundsDictionary.ContainsKey(soundType))
            {
                if (SoundsDictionary[soundType].ContainsKey(name))
                {
                    return SoundsDictionary[soundType][name];
                }
                else throw new ArgumentException("Couldn not find sound with this name", "name");
            }
            else throw new ArgumentException("There are no sounds of this type", "soundType");
        }

        public SoundBuffer GetRandomSound(string soundType)
        {
            if (SoundsDictionary.ContainsKey(soundType))
            {
                Random random = new Random();
                return SoundsDictionary[soundType].ElementAt(random.Next(0, SoundsDictionary[soundType].Count)).Value;
            }
            else throw new ArgumentException("There are no sounds of this type", "soundType");
        }

        private void LoadSound()
        {
            SoundsDictionary = new Dictionary<string, Dictionary<string, SoundBuffer>>();

            XDocument soundConfig;

            try
            {
                soundConfig = XDocument.Load("Resources/Config/Sounds.xml");
            }
            catch (Exception)
            {
                throw new Exception("File Sounds.xml couldn't be loaded");
            }

            foreach (FieldInfo fieldInfo in typeof(SoundType).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                try
                {
                    string soundType = fieldInfo.GetValue(null).ToString();
                    SoundsDictionary[soundType] = (from sound in soundConfig.Descendants("sound")
                                                       where sound.Element("type").Value == soundType
                                                       select new
                                                       {
                                                           symbol = sound.Element("name").Value,
                                                           location = sound.Element("location").Value
                                                       }).ToDictionary(result => result.symbol, result => new SoundBuffer(result.location));
                }
                catch (Exception)
                {
                    throw new Exception("Sounds file couldn't be loaded");
                }
            }
        }
    }
}
