using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SFML.Graphics;
using TankGame.Core.Sounds;

namespace TankGame.Core.Textures;

public static class TextureManager {
    static TextureManager()
        => LoadTextures();

    private static Dictionary<TextureType, Dictionary<string, Texture>> TexturesDictionary { get; set; }

    public static Texture GetTexture(TextureType textureType, string name) {
        if (TexturesDictionary.ContainsKey(textureType)) {
            if (TexturesDictionary[textureType]
               .ContainsKey(name))
                return TexturesDictionary[textureType][name];
            throw new ArgumentException("Couldn not find texture with this name", nameof(name));
        }

        throw new ArgumentException("There are no textures of this type", nameof(textureType));
    }

    public static string GetNameFromTexture(TextureType textureType, Texture texture) {
        if (TexturesDictionary.ContainsKey(textureType)) {
            foreach (var StringTexturePair in TexturesDictionary[textureType])
                if (StringTexturePair.Value == texture)
                    return StringTexturePair.Key;

            throw new ArgumentException("Could not find given texture", nameof(texture));
        }

        throw new ArgumentException("There are no textures of this type", nameof(textureType));
    }

    private static void LoadTextures() {
        string json = File.ReadAllText("Resources/Config/Textures.json");
        var textureDtos = JsonSerializer.Deserialize<List<ResourceDto<TextureType>>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        TexturesDictionary = textureDtos.GroupBy(dto => dto.Type)
                                        .ToDictionary(group => group.Key, group => group.ToDictionary(dto => dto.Name, dto => new Texture(dto.Location)));
    }
}