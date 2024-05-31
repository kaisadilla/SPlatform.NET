using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using splatform.game.scenes;
using splatform.utils;
using splatform.window;

namespace splatform.game;

internal class Game : IDrawable {
    private Scene _scene;

    public GameContext GameContext { get; private set; } = new();

    public Game () {

    }

    public void Init (WindowManager windowManager) {
        Listener.GlobalVolume = 50f;
        
        _scene = LevelScene.FromBinary("level1-1-goomba");

        _scene.SetWindowSizes((ivec2)windowManager.Window.Size, windowManager.Zoom);
        _scene.Init(this, windowManager.Window);
        _scene.Start();
    }

    public void Update () {
        _scene.Update();
    }

    public void FixedUpdate () {
        _scene.FixedUpdate();
    }

    public void LateUpdate () {
        _scene.LateUpdate();
    }

    public void End (WindowManager windowManager) {
        _scene.Close(windowManager.Window);
    }

    public void DrawToWindow (RenderWindow window) {
        _scene.DrawToWindow(window);
    }

    // TODO: Implement
    private void HandleSceneKeyEvents (object? sender, KeyEventArgs evt) {
        _scene.ProcessKeyboardEvents(evt);
    }
}
