using splatform.physics;
using splatform.tiles;

namespace splatform.entities;
internal class SuperMushroom : Item {
    const float SPEED_X = 16f * 5f;
    const float SPEED_Y = 16f * 10f;

    public SuperMushroom () {
        _flipSpriteWhenLookingLeft = false;
        _gravityScale = 0f;
    }

    protected override void OnStart () {
        base.OnStart();

        StartMovement(_startingDirectionRight, SPEED_X);
    }

    protected override void OnCollisionWithTile (Collision collision, OldTile tile) {
        WalkAwayFromTile(collision, SPEED_X);
    }

    protected override void OnCollisionWithPlayer (Collision collision, Player player) {
        _isDead = true;
        _despawnTimer = 0f;
        player.SetMode(Player.MarioMode.Big);
    }

    public override void UpdatePhysics (float deltaTime) {
        _velocity.Y = SPEED_Y;
        Move(_velocity * SECONDS_PER_FIXED_UPDATE);
    }
}
