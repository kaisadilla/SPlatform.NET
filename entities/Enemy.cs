using SFML.Audio;
using SFML.Graphics;
using splatform.assets;
using splatform.physics;
using splatform.tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.entities;
internal abstract class Enemy : Entity {
    public override GameObjectType Type => GameObjectType.Enemy;

    #region Sounds
    protected Sound _sound_kick;
    protected Sound _sound_stomp;
    #endregion

    protected bool _dyingWithStyle = false;

    public Enemy () {
        _sound_kick = new(Assets.Sounds!.Kick);
        _sound_stomp = new(Assets.Sounds!.Stomp);
    }

    /// <summary>
    /// Kills this enemy and makes its corpse fall out of the world.
    /// </summary>
    /// <param name="direction"></param>
    public void DieWithStyle (Direction direction) {
        _dyingWithStyle = true;
        _isDead = true;
        _despawnTimer = 3f;

        _ignoresTiles = true;
        _ignoresMobs = true;

        _velocity.X = (direction == Direction.Left) ? 16f * -4f : 16f * 4f;
        _velocity.Y = 16f * -14f;
        _animationSpeed = 0f;
        _flipVertical = true;

        _sound_kick.Play();
    }

    /// <summary>
    /// Returns true if the position of the player relative to this enemy is
    /// considered as stomping this enemy, rather than running into it.
    /// </summary>
    /// <param name="playerCol">The position of the player's collider.</param>
    /// <returns></returns>
    protected virtual bool IsBeingTrampledByPlayer (FloatRect playerCol) {
        // the middle of the player's collider is clearly (+4 margin) above the
        // top of this enemy's collider.
        return (playerCol.Top + playerCol.Height) < (ColliderPosition.Top + 4);
    }
}
