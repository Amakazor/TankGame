using SFML.Graphics;

namespace TankGame.Core.Fonts; 

public class FontFile {
    public static readonly FontFile PressStart2P = new("Resources/Fonts/PressStart2P-Regular.ttf");
    public static readonly FontFile Iosevka = new("Resources/Fonts/iosevka-custom-regular.ttf");
    
    private FontFile(string path) {
        Path = path;
    }
    
    public string Path { get; }
    
    public static implicit  operator Font(FontFile fontFile) {
        return new(fontFile.Path);
    }
}