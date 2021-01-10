using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace TankGame.Src.Data.Textures
{
    internal class TextureManager
    {
        private static TextureManager instance;

        private Dictionary<string, Dictionary<string, Texture>> TexturesDictionary { get; set; }

        private TextureManager()
        {
            LoadTextures();
        }

        public static TextureManager Instance { get { return instance ?? (instance = new TextureManager()); } }

        public static void Initialize() => _ = Instance;

        public Texture GetTexture(string textureType, string name)
        {
            if (TexturesDictionary.ContainsKey(textureType))
            {
                if (TexturesDictionary[textureType].ContainsKey(name)) return TexturesDictionary[textureType][name];
                else throw new ArgumentException("Couldn not find texture with this name", "name");
            }
            else throw new ArgumentException("There are no textures of this type", "textureType");
        }

        public string GetNameFromTexture(string textureType, Texture texture)
        {
            if (TexturesDictionary.ContainsKey(textureType))
            {
                foreach (KeyValuePair<string, Texture> StringTexturePair in TexturesDictionary[textureType])
                {
                    if (StringTexturePair.Value == texture) return StringTexturePair.Key;
                }

                throw new ArgumentException("Could not find given texture", "texture");
            }
            else throw new ArgumentException("There are no textures of this type", "textureType");
        }

        private void LoadTextures()
        {
            TexturesDictionary = new Dictionary<string, Dictionary<string, Texture>>();

            XDocument textureConfig;

            try
            {
                textureConfig = XDocument.Load("Resources/Config/Textures.xml");
            }
            catch (Exception)
            {
                throw new Exception("File Textures.xml couldn't be loaded");
            }

            foreach (FieldInfo fieldInfo in typeof(TextureType).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                try
                {
                    string textureType = fieldInfo.GetValue(null).ToString();
                    TexturesDictionary[textureType] = (from texture in textureConfig.Descendants("texture")
                                                       where texture.Element("type").Value == textureType
                                                       select new
                                                       {
                                                           symbol = texture.Element("name").Value,
                                                           location = texture.Element("location").Value
                                                       }).ToDictionary(result => result.symbol, result => new Texture(result.location) { Smooth = true });
                }
                catch (Exception)
                {
                    throw new Exception("Texture file couldn't be loaded");
                }
            }
        }
    }
}