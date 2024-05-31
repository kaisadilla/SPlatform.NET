using splatform.physics;

namespace splatform.entities;
internal class Item : Entity {
    public override GameObjectType Type => GameObjectType.Item;

    public override void TakeDamage (
        bool forceDeath, Direction direction = Direction.None
    ) {
        // Nothing.
    }
}
