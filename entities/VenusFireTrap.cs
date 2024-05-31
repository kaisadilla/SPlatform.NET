using splatform.physics;

namespace splatform.entities;
internal class VenusFireTrap : Enemy {
    private int _projectileCount = 1;
    private bool _canBeStomped = false;

    public VenusFireTrap (int projectileCount, bool canBeStomped) {
        _projectileCount = projectileCount;
        _canBeStomped = canBeStomped;
    }

    protected override void OnCollisionWithPlayer (Collision collision, Player player) {
        // TODO
    }

    public override void TakeDamage (bool forceDeath, Direction direction = Direction.None) {
        throw new NotImplementedException(); // TODO
    }
}
