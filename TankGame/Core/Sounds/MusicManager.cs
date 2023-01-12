using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SFML.Audio;

namespace TankGame.Core.Sounds;

public static class MusicManager {
    private const string Path = "Resources/Config/Music.json";
    private static Music? CurrentMusic { get; set; }
    private static string? CurrentMusicLocation { get; set; }
    private static MusicType CurrentMusicType { get; set; }
    private static Dictionary<MusicType, Dictionary<string, string>> MusicDictionary { get; } = Load();

    private static Dictionary<MusicType, Dictionary<string, string>> Load() {
        var musicDtos = JsonSerializer.Deserialize<List<ResourceDto<MusicType>>>(File.ReadAllText(Path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return musicDtos!.GroupBy(dto => dto.Type)
                                   .ToDictionary(group => group.Key, group => group.ToDictionary(dto => dto.Name, dto => dto.Location));
    }

    public static void Play(MusicType musicType, string name) {
        if (!MusicDictionary.ContainsKey(musicType) || !MusicDictionary[musicType].TryGetValue(name, out string? value)) throw new ArgumentException($"There is no music of type {musicType}", nameof(musicType));

        if (value == CurrentMusicLocation) return;
        CurrentMusicLocation = value;
        CurrentMusic?.Dispose();
        CurrentMusic = new(CurrentMusicLocation);
        CurrentMusic.Loop = true;
        CurrentMusic.Volume = 10;
        CurrentMusic.Play();
    }

    public static void PlayRandom(MusicType musicType) {
        if (!MusicDictionary.TryGetValue(musicType, out var value)) throw new ArgumentException($"There is no music of type {musicType}", nameof(musicType));

        if (musicType == CurrentMusicType) return;

        CurrentMusicType = musicType;
        CurrentMusic?.Dispose();
        CurrentMusic = new(value.ElementAt(new Random().Next(0, MusicDictionary[musicType].Count)).Value);
        CurrentMusic.Loop = true;
        CurrentMusic.Volume = 5;
        CurrentMusic.Play();
    }

    public static void Stop() {
        CurrentMusic?.Dispose();
        CurrentMusic = null;
        CurrentMusicLocation = null;
        CurrentMusicType = MusicType.None;
    }
}