using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SFML.Graphics;

namespace TankGame.Core.Textures;

public static class TextureManager {
    private const string Path = "Resources/Config/Textures.json";
    private static Dictionary<TextureType, Dictionary<string, Texture>> TexturesDictionary { get; } = Load();

    public static Texture Get(TextureType textureType, string name) {
        if (TexturesDictionary.TryGetValue(textureType, out var textures)) {
            if (textures.TryGetValue(name, out Texture? texture)) return texture;
            throw new ArgumentException($"Could not find texture {texture}", nameof(name));
        }
        throw new ArgumentException($"There are no textures of type {textureType}", nameof(textureType));
    }

    public static string GetName(TextureType textureType, Texture texture) {
        if (TexturesDictionary.TryGetValue(textureType, out var textures))
            return textures
                  .FirstOrDefault(x => x.Value == texture).Key 
                   ?? throw new ArgumentException($"Could not find texture {texture}", nameof(texture));

        throw new ArgumentException($"There are no textures of type {textureType}", nameof(textureType));
    }

    private static Dictionary<TextureType, Dictionary<string, Texture>> Load() {
        string json = File.ReadAllText(Path);
        var textureDtos = JsonSerializer.Deserialize<List<ResourceDto<TextureType>>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return textureDtos!.GroupBy(dto => dto.Type).ToDictionary(group => group.Key, group => group.ToDictionary(dto => dto.Name, dto => new Texture(dto.Location)));
    }
}