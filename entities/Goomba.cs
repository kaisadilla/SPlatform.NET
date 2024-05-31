using SFML.Graphics;
using splatform.entities.traits;
using splatform.physics;
using splatform.tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.entities;
internal class Goomba : Enemy {

    private const float WALKING_SPEED = 32f;

    public enum AnimStates {
        WALKING,
        STOMPED,
    }

    public Goomba (bool avoidsCliffs) {
        _traits.Add(new AvoidCliffsTrait(this, avoidsCliffs, _size.Y)); // TODO: Size is not yet defined at this point.
    }

    protected override void OnStart () {
        base.OnStart();
        
        StartMovement(_startingDirectionRight, WALKING_SPEED);
    }

    protected override void OnFixedUpdate () {
        base.OnFixedUpdate();
    }

    protected override void OnCollisionWithTile (
        Collision collision, Tile tile
    ) {
        WalkAwayFromTile(collision, WALKING_SPEED);
    }

    protected override void OnCollisionWithEnemy (
        Collision collision, Enemy enemy
    ) {
        WalkAwayFromEntity(collision, WALKING_SPEED);
    }

    protected override void OnCollisionWithPlayer (
        Collision collision, Player player
    ) {
        if (_isDead) return;

        if (IsBeingTrampledByPlayer(player.ColliderPosition)) {
            TakeDamage(false);
            _sound_stomp.Play();
            player.JumpWithStompStrength();
        }
        else {
            player.TakeDamage(false);
        }
    }

    public override void TakeDamage (
        bool forceDeath, Direction direction = Direction.None
    ) {
        if (forceDeath) {
            DieWithStyle(direction);
        }
        else {
            _animations.SetState((int)AnimStates.STOMPED);
            _velocity.X = 0;

            _isDead = true;
            _despawnTimer = DEAD_ENEMY_DESPAWN_TIME;
        }
    }

    public override void Die () {
        base.Die();
        _animations.SetState((int)AnimStates.STOMPED);
        _velocity.X = 0;

        JobManager.AddJob(.25f, Dispose);
    }
}
