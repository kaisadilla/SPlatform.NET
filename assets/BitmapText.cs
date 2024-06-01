using SFML.Graphics;
using splatform.window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.assets;
internal class BitmapText : Drawable {
    public enum TextAlign {
        Left,
        Right,
    };

    public BitmapFont Font { get; set; }
    public TextAlign Align { get; set; } = TextAlign.Left;
    public vec2 Scale { get; private set; } = new(1, 1);
    public vec2 Pivot { get; set; } = new(0, 0);

    private List<Sprite> _stringSprites = new();

    public BitmapText (BitmapFont font) {
        Font = font;
    }

    #region Setters
    public void SetScale (vec2 scale) {
        Scale = scale;
    }

    public void SetScale (float scale) {
        Scale = new(scale, scale);
    }
    #endregion

    public void SetString (string str) {
        _stringSprites.Clear();

        str = str.ToUpperInvariant();
        int xOffset = 0;

        if (Align == TextAlign.Right) {
            foreach (char c in str) {
                if (Font.TryGetCharacterRect(c, out var rect)) {
                    xOffset -= (int)(rect.Width * Scale.X);
                }
            }
        }

        foreach (char c in str) {
            if (Font.TryGetCharacterRect(c, out var rect) == false) {
                continue;
            }

            Sprite charSprite = new(Font.Texture) {
                Scale = Scale,
                TextureRect = rect,
                Position = new(Pivot.X + xOffset, Pivot.Y)
            };

            xOffset += (int)(rect.Width * Scale.X);

            _stringSprites.Add(charSprite);
        }
    }

    public void SetString<T> (T obj) {
        SetString(obj?.ToString() ?? "null");
    }

    public void Draw (RenderTarget target, RenderStates states) {
        foreach (var sprite in _stringSprites) {
            target.Draw(sprite, states);
        }
    }
}
