using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFML.Graphics;
using splatform.assets.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.assets;
internal class BitmapFont {
    public Texture Texture { get; private set; }
    private Dictionary<char, IntRect> _chars;
    private vec2 _spacing;

    public BitmapFont (string filePath) {
        string stringContent = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<BitfontData>(stringContent);

        if (data == null) {
            throw new Exception($"Couldn't parse '{filePath}' json data.");
        }

        _chars = new();
        _spacing = data.Spacing.ToVec2();

        var sheetDims = data.SpritesheetDimensions.ToIvec2();
        var cellDims = data.CellDimensions.ToIvec2();
        IntRect cellOffset = data.CellOffset.ToIntRect();

        foreach (var charData in data.Characters) {
            var charOffset = charData.Offset?.ToIntRect() ?? cellOffset;

            int row = charData.Index / sheetDims.X;
            int col = charData.Index % sheetDims.X;

            int left = (col * cellDims.X) + charOffset.Left;
            int top = (row * cellDims.X) + charOffset.Top;
            int width = cellDims.X - charOffset.Left - charOffset.Width;
            int height = cellDims.Y - charOffset.Top - charOffset.Height;

            _chars[charData.Character] = new(left, top, width, height);
        }

        Texture = new(PATH_BITFONT_SPRITES + "/" + data.Spritesheet + ".png");
    }

    /// <summary>
    /// Returns the IntRect of the given character in this font's texture.
    /// </summary>
    /// <param name="character">The character to look for</param>
    public IntRect GetCharacterRect (char character) {
        return _chars[character];
    }

    /// <summary>
    /// Outputs the IntRect of the given character.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="characterRect"></param>
    /// <returns>True if this font has that character, or false otherwise.</returns>
    public bool TryGetCharacterRect (char character, out IntRect characterRect) {
        return _chars.TryGetValue(character, out characterRect);
    }
}
