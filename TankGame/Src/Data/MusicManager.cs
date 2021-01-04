using SFML.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace TankGame.Src.Data
{
    internal class MusicManager
    {
        private static MusicManager instance;
        private Music CurrentMusic { get; set; }
        private string CurrentMusicLocation { get; set; }
        private string CurrentMusicType { get; set; }
        private Dictionary<string, Dictionary<string, string>> MusicDictionary;

        private MusicManager()
        {
            LoadMusic();
            CurrentMusic = null;
            CurrentMusicLocation = null;
        }

        public static MusicManager Instance { get { return instance ?? (instance = new MusicManager()); } }
        public static void Initialize() => _ = Instance;

        private void LoadMusic()
        {
            MusicDictionary = new Dictionary<string, Dictionary<string, string>>();

            XDocument musicConfig;

            try
            {
                musicConfig = XDocument.Load("Resources/Config/Music.xml");
            }
            catch (Exception)
            {
                throw new Exception("File Music.xml couldn't be loaded");
            }

            foreach (FieldInfo fieldInfo in typeof(MusicType).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                try
                {
                    string musicType = fieldInfo.GetValue(null).ToString();
                    MusicDictionary[musicType] = (from music in musicConfig.Root.Descendants("music")
                                                   where music.Element("type").Value == musicType
                                                   select new
                                                   {
                                                       symbol = music.Element("name").Value,
                                                       location = music.Element("location").Value
                                                   }).ToDictionary(result => result.symbol, result => result.location);
                }
                catch (Exception)
                {
                    throw new Exception("Music file couldn't be loaded");
                }
            }

            MusicDictionary.ToList().ForEach(musicType
                => musicType.Value.ToList().ForEach(musicElement
                    =>
                        {
                            if(!File.Exists(musicElement.Value)) throw new Exception("Music file couldn't be loaded");
                        }));
        }

        public void PlayMusic(string musicType, string name)
        {
            if (MusicDictionary.ContainsKey(musicType))
            {
                if (MusicDictionary[musicType].ContainsKey(name))
                {
                    string newMusicLocation = MusicDictionary[musicType][name];

                    if (newMusicLocation != CurrentMusicLocation)
                    {
                        CurrentMusicLocation = newMusicLocation;
                        if (CurrentMusic != null) CurrentMusic.Dispose();
                        CurrentMusic = new Music(CurrentMusicLocation)
                        {
                            Loop = true,
                            Volume = 10
                        };
                        CurrentMusic.Play();
                    }

                }
                else throw new ArgumentException("There is no music of this name", "musicType");
            }
            else throw new ArgumentException("There is no music of this type", "musicType");
        }

        public void PlayRandomMusic(string musicType)
        {
            if (MusicDictionary.ContainsKey(musicType))
            {
                if (musicType != CurrentMusicType)
                { 
                    CurrentMusicType = musicType;
                    if (CurrentMusic != null) CurrentMusic.Dispose();
                    CurrentMusic = new Music(MusicDictionary[musicType].ElementAt(new Random().Next(0, MusicDictionary[musicType].Count)).Value)
                    {
                        Loop = true,
                        Volume = 10
                    };
                    CurrentMusic.Play();
                }
            }
            else throw new ArgumentException("There is no music of this type", "musicType");
        }

        public void StopMusic()
        {
            if (CurrentMusic != null) CurrentMusic.Dispose();
            CurrentMusic = null;
            CurrentMusicLocation = null;
        }
    }
}
