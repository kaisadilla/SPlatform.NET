using SFML.Graphics;
using SFML.Window;
using splatform.utils;
using splatform.window.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.window;
internal class WindowManager {
    private const int WINDOW_WIDTH = WINDOW_WIDTH_IN_TILES * PIXELS_PER_TILE;
    private const int WINDOW_HEIGHT = WINDOW_HEIGHT_IN_TILES * PIXELS_PER_TILE;

    private const int DEFAULT_FPS_TARGET = 180;
    private const int NO_FPS_TARGET = 0;
    private const float FPS_INFO_UPDATE_TIME = 0.1f; // in seconds

    /// <summary>
    /// The window currently being managed by this object.
    /// </summary>
    public RenderWindow Window { get; private set; }
    public FpsCounter FpsCounter { get; private set; } = new();
    public DebugPanel _debugPanel = new();
    /// <summary>
    /// The target FPS last set for the window. Changing this property does
    /// not update the window to a new target.
    /// </summary>
    public int LastTargetFps { get; private set; } = DEFAULT_FPS_TARGET;
    /// <summary>
    /// The zoom of the window. All elements of the game (notably excluding
    /// debug info) will be rendered at the specified zoom.
    /// </summary>
    public int Zoom { get; private set; }

    #region Helpers
    public bool IsOpen => Window.IsOpen;
    #endregion

    public WindowManager (int initialZoom) {
        Zoom = initialZoom;
        Window = new(
            new(WINDOW_WIDTH * (uint)Zoom, WINDOW_HEIGHT * (uint)Zoom),
            WINDOW_TITLE,
            Styles.Close
        );

        Window.SetFramerateLimit(DEFAULT_FPS_TARGET);
        Window.Closed += (sender, evt) => ((RenderWindow?)sender)?.Close();
        Window.KeyPressed += HandleKeyPressed;

        FpsCounter.SetUpdateTime(FPS_INFO_UPDATE_TIME);
    }

    #region Setters
    /// <summary>
    /// Sets the fps target for this window and updates the window to reflect
    /// that.
    /// </summary>
    /// <param name="amount"></param>
    public void SetTargetFps (int amount) {
        LastTargetFps = amount;
        Window.SetFramerateLimit((uint)amount);
    }
    #endregion

    public void NextFrame () {
        FpsCounter.Count();
        Window.DispatchEvents();

        //Window.SetTitle($"SPlatform - {FpsCounter.Fps} fps [{FpsCounter.Latency * 1000f} ms]");
    }

    public void Draw (params IDrawable[] drawables) {
        Window.Clear();

        foreach (var drawable in drawables) {
            drawable.DrawToWindow(Window);
        }

        if (Debug.DisplayStats) {
            _debugPanel.DrawPanel(Window);
        }

        Window.Display();
    }

    public void Close () {
        Window.Close();
    }

    public void HandleKeyPressed (object? sender, KeyEventArgs evt) {
        if (evt.Code == Keyboard.Key.F1) {
            Time.PauseOrResume();
        }
        // Change key so it's not easy to press accidentally.
        else if (evt.Code == Keyboard.Key.F2) {
            // restart level
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
            // if fps are not capped, cap it to the default target.
            if (LastTargetFps == NO_FPS_TARGET) {
                SetTargetFps(DEFAULT_FPS_TARGET);
            }
            // else, uncap them.
            else {
                SetTargetFps(NO_FPS_TARGET);
            }
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
    }
}
