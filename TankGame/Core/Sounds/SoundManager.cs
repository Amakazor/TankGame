using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SFML.Audio;
using SFML.System;
using TankGame.Core.Gamestate;
using TankGame.Extensions;

namespace TankGame.Core.Sounds;

public static class SoundManager {
    private const string Path = "Resources/Config/Sounds.json";
    private static Dictionary<SoundType, Dictionary<string, SoundBuffer>> SoundsDictionary { get; } = Load();
    private static List<Sound> Sounds { get; } = new();

    private static SoundBuffer Get(SoundType soundType, string name) {
        if (!SoundsDictionary.TryGetValue(soundType, out var sounds))
            throw new ArgumentException("There are no sounds of this type", nameof(soundType));

        if (!sounds.TryGetValue(name, out var value))
            throw new ArgumentException($"Could not find sound with name {name}", nameof(name));

        return value;
    }

    private static SoundBuffer Get(SoundType soundType) {
        if (!SoundsDictionary.TryGetValue(soundType, out var value)) throw new ArgumentException("There are no sounds of this type", nameof(soundType));

        var random = new Random();
        return value.ElementAt(random.Next(0, SoundsDictionary[soundType].Count)).Value;
    }

    private static Dictionary<SoundType, Dictionary<string, SoundBuffer>> Load() {
        var soundDtos = JsonSerializer.Deserialize<List<ResourceDto<SoundType>>>(File.ReadAllText(Path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (soundDtos == null) return new();

        return soundDtos.GroupBy(dto => dto.Type)
                        .ToDictionary(group => group.Key, group => group.ToDictionary(dto => dto.Name, dto => new SoundBuffer(dto.Location)));
    }

    private static void Clear() {
        foreach (Sound sound in Sounds.ToList().Where(sound => sound.Status == SoundStatus.Stopped)) {
            Sounds.Remove(sound);
            sound.Dispose();
        }
    }

    public static void Play(SoundType soundType, string name, Vector2f position)
        => PlayFromBuffer(soundType, Get(soundType, name), position);

    public static void PlayRandom(SoundType soundType, Vector2f position)
        => PlayFromBuffer(soundType, Get(soundType), position);

    private static void PlayFromBuffer(SoundType soundType, SoundBuffer soundbuffer, Vector2f position) {
        GamestateManager.Player.IfSome(
            player => {
                Clear();

                float soundDistance = position.ManhattanDistance(player.Coords);

                float volume = Math.Min(20 * 1 / (soundDistance == 0 ? 1 : soundDistance), 20) * (soundType == SoundType.Move ? 0.25F : 1);

                if (!(volume > 0.25)) return;

                var newSound = new Sound(soundbuffer);
                newSound.Volume = volume;
                newSound.Play();

                Sounds.Add(newSound);
            }
        );
    }
}