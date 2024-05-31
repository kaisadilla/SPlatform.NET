using SFML.Graphics;
using SFML.Window;

namespace splatform.game.scenes;
internal abstract class Scene {
    protected Game _game; // TODO: Isolate the part of Game needed by this class to GameContext.

    // TODO: Remove both of these from here, keep it in Game class.
    public ivec2 WindowSize { get; protected set; }
    public vec2 WindowZoom { get; protected set; }

    /// <summary>
    /// Initializes all the necessary objects to run the scene.
    /// </summary>
    /// <param name="game">The game that runs this scene.</param>
    /// <param name="window">The window where this scene will be played.</param>
    public abstract void Init(Game game, RenderWindow window);
    /// <summary>
    /// A method called only once, at the very start of the scene.
    /// Dispatches events that should occur on the start of the scene, such as
    /// playing the background music.
    /// </summary>
    public abstract void Start();
    /// <summary>
    /// A method called every frame of the program.
    /// </summary>
    public abstract void Update();
    /// <summary>
    /// A method called every fixed update of the program (that is, the update
    /// that manages physics and other fps-independent calculations).
    /// </summary>
    public abstract void FixedUpdate();
    /// <summary>
    /// A method called every frame of the program, AFTER Update() and, ideally,
    /// after things have been drawn to the screen.
    /// </summary>
    public abstract void LateUpdate();
    /// <summary>
    /// Draws the contents of the scene to the given window.
    /// </summary>
    /// <param name="window">The window to draw to.</param>
    public abstract void DrawToWindow(RenderWindow window);
    /// <summary>
    /// Processes keyboard events. Note that some input is read directly from
    /// the keyboard each update and not triggered by keyboard events.
    /// </summary>
    /// <param name="evt">The keyboard event that triggered this call.</param>
    public abstract void ProcessKeyboardEvents(KeyEventArgs evt);

    /// <summary>
    /// Tidies things up to allow this scene to be discarded safely.
    /// </summary>
    /// <param name="window">The window where this scene is being shown.</param>
    public abstract void Close(RenderWindow window);

    /// <summary>
    /// Sets the size and zoom of the current window.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="zoom"></param>
    public void SetWindowSizes (ivec2 size, vec2 zoom) {
        WindowSize = size;
        WindowZoom = zoom;
    }
}
