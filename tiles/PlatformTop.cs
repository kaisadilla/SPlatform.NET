using splatform.physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.tiles;
internal class PlatformTop : Tile {
    public override bool HasMobCollided (Collision collision, vec2 mobVelocity) {
        return collision.Direction == Direction.Down && mobVelocity.Y > 0f;
    }
}
