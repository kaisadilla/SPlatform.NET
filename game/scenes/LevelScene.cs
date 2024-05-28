using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using splatform.player;
using splatform.tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.game.scenes;
internal partial class LevelScene : Scene {
    public int Width { get; private set; }
    public int Height { get; private set; }


    /************************
     * BACKGROUND AND MUSIC *
     ************************/
    private float _backgroundParallaxMaxX = 0;
    private float _backgroundParallaxMaxY = 0;
    private float _levelParallaxMaxX = 0;
    private float _levelParallaxMaxY = 0;
    private Texture _backgroundTexture = new(1, 1);
    private Sprite _backgroundImage = new();
    private Music? _music;

    private List<Tile> _backgroundLayer = new();
    private List<Tile> _foregroundLayer = new();
    private List<Tile> _detailLayer = new();
    private List<Tile> _frontLayer = new();

    private Camera _camera = new(new(1, 1), new(1, 1));
    private vec2 __temp_playerPos = new(0, 400);

    public int GridWidth => Width;
    public int GridHeight => Height;
    public int PixelWidth => Width * PIXELS_PER_TILE;
    public int PixelHeight => Height * PIXELS_PER_TILE;

    /// <summary>
    /// The size, in pixels, of the view. This is the amount of in-game pixels
    /// that can be seen in the window, not the amount of screen pixels. For
    /// example, if zoom is equal to 2, then each in-game pixel takes 4 screen
    /// pixels. In practice, this is equal to WindowSize / WindowZoom.
    /// </summary>
    public ivec2 ViewSize => new(
        (int)(WindowSize.X / WindowZoom.X),
        (int)(WindowSize.Y / WindowZoom.Y)
    );

    public override void Init (RenderWindow window) {
        InitializeBackground();

        _camera = new(new(PixelWidth, PixelHeight), ViewSize);
    }

    public override void OnEnter () {

        foreach (var tile in _backgroundLayer) {
            tile.OnStart();
        }
        foreach (var tile in _foregroundLayer) {
            tile.OnStart();
        }
    }

    public override void OnUpdate () {
        _camera.UpdatePosition(__temp_playerPos);

        UpdateBackgroundPosition();

        // TODO: Remove
        if (Keyboard.IsKeyPressed(Keyboard.Key.A)) {
            __temp_playerPos.X -= 1;
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.D)) {
            __temp_playerPos.X += 1;
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.W)) {
            __temp_playerPos.Y -= 1;
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.S)) {
            __temp_playerPos.Y += 1;
        }
    }

    public override void OnFixedUpdate () {
        throw new NotImplementedException();
    }

    public override void OnLateUpdate () {
        throw new NotImplementedException();
    }

    public override void OnDraw (RenderWindow window) {
        window.Draw(_backgroundImage);

        window.SetView(_camera.View);
        DrawLayer(window, _backgroundLayer);
        DrawLayer(window, _foregroundLayer);

        window.SetView(window.DefaultView);
    }

    public override void OnEvent (KeyEventArgs evt) {

    }

    public override void Dispatch (RenderWindow window) {

    }

    private void LoadBackground (string name) {
        _backgroundTexture = new(PATH_BACKGROUND_IMAGES + "/" + name + ".png");
        _backgroundImage.Texture = _backgroundTexture;
        _backgroundImage.Scale = new(2f, 2f); // TODO: window scale
    }

    private void LoadMusic (string name) {
        _music = new(PATH_MUSIC + "/" + name + ".wav");
        _music.Loop = true;
        _music.Volume = 50f; // TODO: Dynamic??
    }

    private void InitializeBackground () {
        // TODO: This is hardcoded to scale to 2x zoom.
        var bgSize = _backgroundTexture.Size;
        // TODO: Will underflow if the size of the window is bigger than the
        // size of the background texture. / ???
        _backgroundParallaxMaxX = bgSize.X - (WindowSize.X / 2);
        _backgroundParallaxMaxY = bgSize.Y - (WindowSize.Y / 2);
        _levelParallaxMaxX = PixelWidth - (WindowSize.X / 2);
        _levelParallaxMaxY = PixelHeight - (WindowSize.Y / 2);
    }

    private void UpdateBackgroundPosition () {
        var topLeft = _camera.TopLeft;

        int xTop = (int)float.Lerp(
            0, _backgroundParallaxMaxX, topLeft.X / _levelParallaxMaxX
        );
        int yTop = (int)float.Lerp(
            0, _backgroundParallaxMaxY, topLeft.Y / _levelParallaxMaxY
        );

        _backgroundImage.TextureRect = new(
            xTop, yTop, WindowSize.X / 2, WindowSize.Y / 2
         );
    }

    /// <summary>
    /// Draws one layer of tiles to the window given.
    /// </summary>
    /// <param name="window">The window in which to draw the tile.</param>
    /// <param name="layer">The tile layer to draw.</param>
    private void DrawLayer (RenderWindow window, List<Tile> layer) {
        foreach (var tile in layer) {
            tile.Draw(window);
        }
    }
}
