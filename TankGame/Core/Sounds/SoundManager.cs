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
    static SoundManager() {
        LoadSound();
        Sounds = new();
    }

    private static Dictionary<string, Dictionary<string, SoundBuffer>> SoundsDictionary { get; set; }
    private static List<Sound> Sounds { get; }

    private static SoundBuffer GetSound(string soundType, string name) {
        if (!SoundsDictionary.ContainsKey(soundType)) throw new ArgumentException("There are no sounds of this type", nameof(soundType));

        if (!SoundsDictionary[soundType]
               .TryGetValue(name, out SoundBuffer value))
            throw new ArgumentException($"Could not find sound with name {name}", nameof(name));

        return value;
    }

    private static SoundBuffer GetSound(string soundType) {
        if (!SoundsDictionary.TryGetValue(soundType, out var value)) throw new ArgumentException("There are no sounds of this type", nameof(soundType));

        var random = new Random();
        return value.ElementAt(
                         random.Next(
                             0, SoundsDictionary[soundType]
                                .Count
                         )
                     )
                    .Value;
    }

    private static void LoadSound() {
        var soundDtos = JsonSerializer.Deserialize<List<ResourceDto<string>>>(File.ReadAllText("Resources/Config/Sounds.json"), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        SoundsDictionary = soundDtos.GroupBy(dto => dto.Type)
                                    .ToDictionary(group => group.Key, group => group.ToDictionary(dto => dto.Name, dto => new SoundBuffer(dto.Location)));
    }

    private static void ClearSounds() {
        foreach (Sound sound in Sounds.ToList()
                                      .Where(sound => sound.Status == SoundStatus.Stopped)) {
            Sounds.Remove(sound);
            sound.Dispose();
        }
    }

    public static void PlaySound(string soundType, string name, Vector2f position)
        => PlaySoundFromBuffer(soundType, GetSound(soundType, name), position);

    public static void PlayRandomSound(string soundType, Vector2f position)
        => PlaySoundFromBuffer(soundType, GetSound(soundType), position);

    private static void PlaySoundFromBuffer(string soundType, SoundBuffer soundbuffer, Vector2f position) {
        ClearSounds();

        float soundDistance = position.ManhattanDistance(GamestateManager.Player.Position / 64);

        float volume = Math.Min(20 * 1 / (soundDistance == 0 ? 1 : soundDistance), 20) * (soundType == "move" ? 0.25F : 1);

        if (!(volume > 0.25)) return;

        var newSound = new Sound(soundbuffer);
        newSound.Volume = volume;
        newSound.Play();

        Sounds.Add(newSound);
    }
}