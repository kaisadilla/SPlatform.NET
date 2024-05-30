using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.entities.traits;
internal abstract class Trait {
    public virtual void Start () { }
    public virtual void Update () { }
    public virtual void FixedUpdate () { }
    public virtual void LateUpdate () { }
    public virtual void Draw (RenderWindow window) { }
    public virtual void DrawDebugInfo (RenderWindow window) { }
}
