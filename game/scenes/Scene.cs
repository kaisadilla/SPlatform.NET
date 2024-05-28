using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.game.scenes;
internal abstract class Scene {
    public ivec2 WindowSize { get; protected set; }
    public vec2 WindowZoom { get; protected set; }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnFixedUpdate();
    public abstract void OnLateUpdate();
    public abstract void OnDraw(RenderWindow window);
    //public abstract void OnEvent();

    public void SetWindowSizes (ivec2 size, vec2 zoom) {
        WindowSize = size;
        WindowZoom = zoom;
    }
}
