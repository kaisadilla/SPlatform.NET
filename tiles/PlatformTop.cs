using splatform.physics;

namespace splatform.tiles;
internal class PlatformTop : OldTile {
    public override bool HasMobCollided (Collision collision, vec2 mobVelocity) {
        return collision.Direction == Direction.Down && mobVelocity.Y > 0f;
    }
}
