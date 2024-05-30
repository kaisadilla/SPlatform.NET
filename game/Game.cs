using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using splatform.game.scenes;
using splatform.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.game;
// TODO: Split this class into a "Game" class that manages the actual game,
// and some "Program" class that manages windows, options like scale, etc.
internal class Game {
    private const int FPS_CAP = 180; // TODO: parameterize
    private const int WINDOW_WIDTH = WINDOW_WIDTH_IN_TILES * PIXELS_PER_TILE;
    private const int WINDOW_HEIGHT = WINDOW_HEIGHT_IN_TILES * PIXELS_PER_TILE;

    public bool IsOpen => _window.IsOpen;

    #region Player stats
    public int Lives { get; private set; } = 5;
    public int Coins { get; private set; } = 0;
    public int Score { get; private set; } = 0;
    public int YoshiCoins {  get; private set; } = 0;
    public int[] EndLevelItems { get; private set; } = [0, 0, 0];
    #endregion

    private RenderWindow _window;
    private float _cumulativeFixedTime = 0.0f;

    private Scene _scene;

    #region Fps counter fields
    // TODO: Move this functionality to FpsCounter itself.
    private bool _cappedFps = true;
    private FpsCounter _fpsCounter = new();
    private float _timeSinceLastFpsUpdate = 0.0f;
    #endregion

    // todo: move to a class that draws debug info.
    private RectangleShape _debugBg = new();
    private Font _debugFont;
    private Text _infoFps = new();
    private Text _infoTime = new();
    private Text _infoTimeScale = new();

    public Game () {
        _window = new(
            new(WINDOW_WIDTH, WINDOW_HEIGHT),
            "SPlatform",
            SFML.Window.Styles.Close
        );

        _debugFont = new("res/fonts/CascadiaMono.ttf");
    }

    public void Init () {
        VideoMode video = new(WINDOW_WIDTH * 2, WINDOW_HEIGHT * 2);

        _fpsCounter.SetUpdateTime(0.1f);

        // TODO: Is it necessary to make a new window?
        _window.Close();
        _window = new(video, WINDOW_TITLE, SFML.Window.Styles.Close);
        _window.SetFramerateLimit(FPS_CAP);

        SetupWindowEvents();
        SetupDebugInfo();

        Listener.GlobalVolume = 50f;
        
        _scene = LevelScene.FromBinary("level1-1-goomba");

        _scene.SetWindowSizes((ivec2)_window.Size, new(2f, 2f));
        _scene.Init(this, _window);
        _scene.Start();
    }

    public void Update () {
        _cumulativeFixedTime += Time.DeltaTime;

        while (_cumulativeFixedTime > SECONDS_PER_FIXED_UPDATE) {
            FixedUpdate();
            _cumulativeFixedTime -= SECONDS_PER_FIXED_UPDATE;
        }

        UpdateFps();
        _window.DispatchEvents();

        _scene.Update();
    }

    private void FixedUpdate () {
        _scene.FixedUpdate();
    }

    public void LateUpdate () {
        _scene.LateUpdate();
    }

    public void Close () {
        _scene.Close(_window);
        _window.Close();
    }

    public void Draw () {
        _window.Clear();
        _scene.DrawToWindow(_window);

        if (Debug.DisplayStats) {
            DrawDebugInfo();
        }

        _window.Display();
    }

    private void SetupWindowEvents () {
        _window.Closed += (sender, evt) => ((RenderWindow?)sender)?.Close();

        _window.KeyPressed += HandleKeyPressed;
        //_window.KeyPressed += HandleSceneKeyEvents;
    }

    private void HandleKeyPressed (object? sender, KeyEventArgs evt) {
        if (evt.Code == Keyboard.Key.F1) {
            Time.PauseOrResume();
        }
        // TODO: Remove from here, maybe?
        else if (evt.Code == Keyboard.Key.F2) {
            _scene.Close(_window);
            Init(); // TODO This is temporary, Init() should never be called outside main.cs.
        }
        else if (evt.Code == Keyboard.Key.F3) {
            Debug.DisplayStats = !Debug.DisplayStats;
        }
        else if (evt.Code == Keyboard.Key.F4) {
            Debug.DrawColliders = !Debug.DrawColliders;
        }
        else if (evt.Code == Keyboard.Key.F5) {
            Debug.DrawVisualInfo = !Debug.DrawVisualInfo;
        }
        else if (evt.Code == Keyboard.Key.F10) {
            if (_cappedFps) {
                _window.SetFramerateLimit(0);
            }
            else {
                _window.SetFramerateLimit(FPS_CAP);
            }

            _cappedFps = !_cappedFps;
        }
        else if (evt.Code == Keyboard.Key.F11) {
            if (Time.TimeScale == 1f) {
                Time.SetTimeScale(0.5f);
            }
            else if (Time.TimeScale == 0.5f) {
                Time.SetTimeScale(0.25f);
            }
            else if (Time.TimeScale == 0.25f) {
                Time.SetTimeScale(0.125f);
            }
            else {
                Time.SetTimeScale(1f);
            }
        }

        HandleSceneKeyEvents(sender, evt);
    }

    private void HandleSceneKeyEvents (object? sender, KeyEventArgs evt) {
        _scene.ProcessKeyboardEvents(evt);
    }

    private void SetupDebugInfo () {
        _debugBg.FillColor = new(0, 0, 0, 196);
        _debugBg.Position = new(5f, 3f);
        _debugBg.Size = new(220f, 42f);

        _infoFps.Font = _debugFont;
        _infoFps.DisplayedString = $"FPS: 0";
        _infoFps.Position = new(10f, 4f);
        _infoFps.CharacterSize = 12;
        _infoFps.FillColor = Color.White;
        _infoFps.Style = Text.Styles.Regular;

        _infoTime.Font = _debugFont;
        _infoTime.DisplayedString = "Time: 0.00";
        _infoTime.Position = new(10f, 16f);
        _infoTime.CharacterSize = 12;
        _infoTime.FillColor = Color.White;
        _infoTime.Style = Text.Styles.Regular;

        _infoTimeScale.Font = _debugFont;
        _infoTimeScale.DisplayedString = "Time scale: 0.00 [paused]";
        _infoTimeScale.Position = new(10f, 28f);
        _infoTimeScale.CharacterSize = 12;
        _infoTimeScale.FillColor = Color.White;
        _infoTimeScale.Style = Text.Styles.Regular;
    }

    private void UpdateFps () {
        _fpsCounter.Count();

        if (_fpsCounter.UpdatedLastFrame) {
            _infoFps.DisplayedString = $"FPS: {_fpsCounter.Fps} " +
                (_cappedFps ? "[capped]" : "");

            //_window.SetTitle($"SPlatform - {_fpsCounter.Fps} fps [{_fpsCounter.Latency * 1000f} ms]");
        }
    }

    private void DrawDebugInfo () {
        _infoTime.DisplayedString = $"Time: {Time.CurrentTime}";
        _infoTimeScale.DisplayedString = $"Time scale: {Time.TimeScale} " +
            (Time.Paused ? "[paused]" : "");

        _window.Draw(_debugBg);
        _window.Draw(_infoFps);
        _window.Draw(_infoTime);
        _window.Draw(_infoTimeScale);
    }
}
