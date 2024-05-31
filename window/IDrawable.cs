using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.window;
internal interface IDrawable {
    public void DrawToWindow(RenderWindow window);
}
