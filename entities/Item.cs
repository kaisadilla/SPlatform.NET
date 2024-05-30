using splatform.physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.entities;
internal class Item : Entity {
    public override GameObjectType Type => GameObjectType.Item;

    public override void TakeDamage (
        bool forceDeath, Direction direction = Direction.None
    ) {
        // Nothing.
    }
}
