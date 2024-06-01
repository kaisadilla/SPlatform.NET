using SFML.Graphics;
using SFML.System;
using splatform.assets;
using splatform.game;
using splatform.game.scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.window.ui;
internal class LevelUi : IDrawable {
    private BitmapFont _uiFont;
    private Texture _texture;

    private BitmapText _worldIcon;
    private Sprite _livesIcon;
    private Sprite _coinIcon;
    private Sprite _timeIcon;
    private Sprite[] _cards = new Sprite[3];
    private Sprite[] _powerTrackbarIcons = new Sprite[6];
    private Sprite _powerOn;

    private BitmapText _world;
    private BitmapText _lives;
    private BitmapText _coins;
    private BitmapText _time;
    private BitmapText _score;

    public LevelUi (float scale) {
        _uiFont = new(PATH_BITFONTS + "/smb3-font.json");
        _texture = new(PATH_UI_SPRITES + "/ui-elements.png");

        _worldIcon = new(_uiFont);
        _worldIcon.SetScale(scale);
        _worldIcon.Pivot = new(18, 10);
        _worldIcon.SetString("WORLD");

        _world = new(_uiFont);
        _world.SetScale(scale);
        _world.Pivot = new(110, 10);
        _world.SetString("999");

        _livesIcon = new(_texture) {
            Scale = new(scale, scale),
            Position = new(170, 12),
            TextureRect = new(0, 1, 20, 7)
        };

        _lives = new(_uiFont);
        _lives.SetScale(scale);
        _lives.Align = BitmapText.TextAlign.Right;
        _lives.Pivot = new(212 + (16 * 2), 10);
        _lives.SetString("99");

        _coinIcon = new(_texture) {
            Scale = new(scale, scale),
            Position = new(624, 10),
            TextureRect = new(28, 0, 7, 8)
        };

        _coins = new(_uiFont);
        _coins.SetScale(scale);
        _coins.Align = BitmapText.TextAlign.Right;
        _coins.Pivot = new(640 + (16 * 2), 10);
        _coins.SetString("99");

        _timeIcon = new(_texture) {
            Scale = new(scale, scale),
            Position = new(686, 12),
            TextureRect = new(20, 1, 8, 7)
        };

        _time = new(_uiFont);
        _time.SetScale(scale);
        _time.Align = BitmapText.TextAlign.Right;
        _time.Pivot = new(704 + (16 * 3), 10);
        _time.SetString("999");

        _score = new(_uiFont);
        _score.SetScale(scale);
        _score.Align = BitmapText.TextAlign.Right;
        _score.Pivot = new(624 + (16 * 3), 34);
        _score.SetString("99999999");

        for (int i = 0; i < _cards.Length; i++) {
            _cards[i] = new(_texture) {
                Scale = new(scale, scale),
                Position = new(454 + (i * 50), 6),
                TextureRect = new(0, 24, 24, 24),
            };
        }

        for (int i = 0; i < _powerTrackbarIcons.Length; i++) {
            _powerTrackbarIcons[i] = new(_texture) {
                Scale = new(scale, scale),
                Position = new(18 + (i * 16), 34),
                TextureRect = new(0, 8, 8, 7),
            };
        }

        _powerOn = new(_texture) {
            Scale = new(scale, scale),
            Position = new(116, 34),
            TextureRect = new(8, 8, 15, 7),
        };
    }

    public void UpdateInfo (GameContext gameCtx, LevelScene scene) {
        //TODO: _world.SetString(???);
        _lives.SetString(gameCtx.Lives);
        _coins.SetString(gameCtx.Coins);
        _time.SetString((int)MathF.Ceiling(scene.TimeLeft));
        _score.SetString(gameCtx.Score);
    }

    public void DrawTo (RenderWindow window) {
        window.Draw(_worldIcon);
        window.Draw(_world);
        window.Draw(_livesIcon);
        window.Draw(_lives);
        window.Draw(_coinIcon);
        window.Draw(_coins);
        window.Draw(_timeIcon);
        window.Draw(_time);
        window.Draw(_score);

        for (int i = 0; i < 3; i++) {
            window.Draw(_cards[i]);
        }

        for (int i = 0; i < 6; i++) {
            window.Draw(_powerTrackbarIcons[i]);
        }

        window.Draw(_powerOn);
    }
}
