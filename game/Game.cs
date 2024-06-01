using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using splatform.game.scenes;
using splatform.utils;
using splatform.window;

namespace splatform.game;

internal class Game : IDrawable {
    public Scene Scene { get; private set; }

    public GameContext Context { get; private set; } = new();

    public Game () {

    }

    public void Init (WindowManager windowManager) {
        Listener.GlobalVolume = 50f;
        
        Scene = LevelScene.FromBinary("level1-1-goomba");

        Scene.SetWindowSizes((ivec2)windowManager.Window.Size, windowManager.Zoom);
        Scene.Init(this, windowManager.Window);
        Scene.Start();
    }

    public void Update () {
        Scene.Update();
    }

    public void FixedUpdate () {
        Scene.FixedUpdate();
    }

    public void LateUpdate () {
        Scene.LateUpdate();
    }

    public void End (WindowManager windowManager) {
        Scene.Close(windowManager.Window);
    }

    public void DrawTo (RenderWindow window) {
        Scene.DrawToWindow(window);
    }

    // TODO: Implement
    private void HandleSceneKeyEvents (object? sender, KeyEventArgs evt) {
        Scene.ProcessKeyboardEvents(evt);
    }
}
