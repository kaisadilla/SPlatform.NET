using SFML.Audio;
using SFML.Graphics;
using splatform.player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.game;
internal class Background {
    private float _bgParallaxMaxX = 0;
    private float _bgParallaxMaxY = 0;
    private float _levelParallaxMaxX = 0;
    private float _levelParallaxMaxY = 0;

    private Texture _backgroundTexture;
    private Sprite _backgroundImage; // TODO: Multiple backgrounds, painted one after another.

    private Music _music;

    private ivec2 _windowSize;
    private ivec2 _sceneSize;

    public Background (string bgName, string musicName, ivec2 sceneSize) {
        _backgroundTexture = new(PATH_BACKGROUND_IMAGES + "/" + bgName + ".png");
        _backgroundImage = new() {
            Texture = _backgroundTexture,
            Scale = new(2f, 2f) // TODO: window scale
        };

        _music = new(PATH_MUSIC + "/" + musicName + ".wav") {
            Loop = true,
            Volume = 50f // TODO: Dynamic??
        };

        _sceneSize = sceneSize;
    }

    public void SetWindowContext (ivec2 windowSize) {
        _windowSize = windowSize;

        // TODO: This is hardcoded to scale to 2x zoom.
        var bgSize = _backgroundTexture.Size;
        // TODO: Will underflow if the size of the window is bigger than the
        // size of the background texture. / ???
        _bgParallaxMaxX = bgSize.X - (windowSize.X / 2);
        _bgParallaxMaxY = bgSize.Y - (windowSize.Y / 2);
        _levelParallaxMaxX = _sceneSize.X - (windowSize.X / 2);
        _levelParallaxMaxY = _sceneSize.Y - (windowSize.Y / 2);
    }

    /// <summary>
    /// Updates the information about the background to draw it according to
    /// the position given.
    /// </summary>
    /// <param name="topLeft">The coordinate currently at the top-left corner
    /// of the window.</param>
    public void Update (vec2 topLeft) {
        int xTop = (int)float.Lerp(
            0, _bgParallaxMaxX, topLeft.X / _levelParallaxMaxX
        );
        int yTop = (int)float.Lerp(
            0, _bgParallaxMaxY, topLeft.Y / _levelParallaxMaxY
        );

        _backgroundImage.TextureRect = new(
            xTop, yTop, _windowSize.X / 2, _windowSize.Y / 2
         );
    }

    /// <summary>
    /// Draws the background to the given window.
    /// </summary>
    /// <param name="window">The window in where to draw the background.</param>
    public void Draw (RenderWindow window) {
        window.Draw(_backgroundImage);
    }

    public void PlayMusic () {
        _music?.Play();
    }

    public void PauseMusic () {
        _music?.Pause();
    }

    public void StopMusic () {
        _music?.Stop();
    }
}
