using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.game.scenes;
internal abstract class Scene {
    // TODO: Remove both of these from here, keep it in Game class.
    public ivec2 WindowSize { get; protected set; }
    public vec2 WindowZoom { get; protected set; }

    public abstract void Init(RenderWindow window);
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnFixedUpdate();
    public abstract void OnLateUpdate();
    public abstract void OnDraw(RenderWindow window);
    public abstract void OnEvent(KeyEventArgs evt);
    /// <summary>
    /// Tidies things up to allow this scene to be discarded safely.
    /// </summary>
    /// <param name="window">The window where this scene is being shown.</param>
    public abstract void Dispatch(RenderWindow window);

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
