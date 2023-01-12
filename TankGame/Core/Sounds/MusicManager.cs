using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SFML.Audio;

namespace TankGame.Core.Sounds;

public static class MusicManager {
    static MusicManager() {
        LoadMusic();
        CurrentMusic = null;
        CurrentMusicLocation = null;
    }

    private static Music CurrentMusic { get; set; }
    private static string CurrentMusicLocation { get; set; }
    private static string CurrentMusicType { get; set; }
    private static Dictionary<string, Dictionary<string, string>> MusicDictionary { get; set; }

    private static void LoadMusic() {
        var musicDtos = JsonSerializer.Deserialize<List<ResourceDto<string>>>(File.ReadAllText("Resources/Config/Music.json"), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        MusicDictionary = musicDtos.GroupBy(dto => dto.Type)
                                   .ToDictionary(group => group.Key, group => group.ToDictionary(dto => dto.Name, dto => dto.Location));
    }

    public static void PlayMusic(string musicType, string name) {
        if (!MusicDictionary.ContainsKey(musicType) || !MusicDictionary[musicType]
               .TryGetValue(name, out string value))
            throw new ArgumentException("There is no music of this type", nameof(musicType));

        if (value == CurrentMusicLocation) return;
        CurrentMusicLocation = value;
        CurrentMusic?.Dispose();
        CurrentMusic = new(CurrentMusicLocation);
        CurrentMusic.Loop = true;
        CurrentMusic.Volume = 10;
        CurrentMusic.Play();
    }

    public static void PlayRandomMusic(string musicType) {
        if (!MusicDictionary.TryGetValue(musicType, out var value)) throw new ArgumentException("There is no music of this type", nameof(musicType));

        if (musicType == CurrentMusicType) return;

        CurrentMusicType = musicType;
        CurrentMusic?.Dispose();
        CurrentMusic = new(
            value.ElementAt(
                      new Random().Next(
                          0, MusicDictionary[musicType]
                             .Count
                      )
                  )
                 .Value
        );
        CurrentMusic.Loop = true;
        CurrentMusic.Volume = 5;
        CurrentMusic.Play();
    }

    public static void StopMusic() {
        CurrentMusic?.Dispose();
        CurrentMusic = null;
        CurrentMusicLocation = null;
        CurrentMusicType = null;
    }
}