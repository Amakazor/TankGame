using SFML.Audio;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using TankGame.Src.Data.Gamestate;
using TankGame.Src.Extensions;

namespace TankGame.Src.Data.Sounds
{
    internal class SoundManager
    {
        private static SoundManager instance;

        private Dictionary<string, Dictionary<string, SoundBuffer>> SoundsDictionary { get; set; }
        private List<Sound> Sounds { get; }

        private SoundManager()
        {
            LoadSound();
            Sounds = new List<Sound>();
        }


        public static SoundManager Instance { get { return instance ?? (instance = new SoundManager()); } }

        public static void Initialize() => _ = Instance;

        private SoundBuffer GetSound(string soundType, string name)
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

        private SoundBuffer GetRandomSound(string soundType)
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

        private void ClearSounds()
        {
            foreach (Sound sound in Sounds.ToList())
            {
                if (sound.Status == SoundStatus.Stopped)
                {
                    Sounds.Remove(sound);
                    sound.Dispose();
                }
            }
        }

        public void PlaySound(string soundType, string name, Vector2f position)
        {
            SoundBuffer soundBuffer = GetSound(soundType, name);
            if (soundBuffer != null) PlaySoundFromBuffer(soundType, soundBuffer, position);

        }
        
        public void PlayRandomSound(string soundType, Vector2f position)
        {
            SoundBuffer soundBuffer = GetRandomSound(soundType);
            if (soundBuffer != null) PlaySoundFromBuffer(soundType, soundBuffer, position);
        }

        private void PlaySoundFromBuffer(string soundType, SoundBuffer soundbuffer, Vector2f position)
        {
            ClearSounds();

            float soundDistance = position.ManhattanDistance(GamestateManager.Instance.Player.Position / 64);

            float volume = Math.Min(20 * 1 / (soundDistance == 0 ? 1 : soundDistance), 20) * ((soundType == "move") ? 0.25F : 1);

            if (volume > 0.25)
            {
                Sound newSound = new Sound(soundbuffer)
                {
                    Volume = volume
                };
                newSound.Play();
                Sounds.Add(newSound);
            }
        }
    }
}