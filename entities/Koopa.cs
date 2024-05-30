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
internal class Koopa : Enemy {
    const float WALKING_SPEED = 32f;
    const float SHELL_SPEED = 32f * 6f;
    const float SECONDS_UNTIL_REVIVE_START = 6f;
    const float SECONDS_UNTIL_REVIVE_END = 1f;

    public enum AnimStates {
        WALKING,
        SHELL,
        SHELL_TRAVELING,
        SHELL_REANIMATING,
        DEAD,
    }

    private bool _canRevive;
    private bool _playerCanGrabShell;

    private IntRect _shellCollider;

    private bool _isShell = false;
    private bool _isReviving = false;
    private float _secondsUntilReviveStart = SECONDS_UNTIL_REVIVE_START;
    private float _secondsUntilReviveEnd = SECONDS_UNTIL_REVIVE_END;

    public Koopa (
        bool avoidsCliffs,
        bool canRevive,
        bool playerCanGrabShell,
        IntRect shellColliderPos
    ) {
        _traits.Add(new AvoidCliffsTrait(this, avoidsCliffs, _size.Y));
        _canRevive = canRevive;
        _playerCanGrabShell = playerCanGrabShell;
        _shellCollider = shellColliderPos;
    }

    protected override void OnStart () {
        base.OnStart();

        StartMovement(_startingDirectionRight, WALKING_SPEED);
    }

    protected override void OnUpdate () {
        base.OnUpdate();

        if (_isReviving) {
            _animationSpeed = 1f + MathF.Pow(1 - _secondsUntilReviveEnd, 2) * 3f;
        }
        else {
            _animationSpeed = 1f;
        }
    }

    protected override void OnFixedUpdate () {
        base.OnFixedUpdate();

        CheckShellRevive();
    }

    protected override void OnCollisionWithTile (
        Collision collision, Tile tile
    ) {
        WalkAwayFromTile(collision, MathF.Abs(_velocity.X));
    }

    protected override void OnCollisionWithEnemy (
        Collision collision, Enemy enemy
    ) {
        if (_isShell) {
            if (IsStill == false) {
                enemy.TakeDamage(
                    true, collision.GetHorizontalDirectionFor(enemy)
                );
            }
        }
        else {
            WalkAwayFromEntity(collision, MathF.Abs(_velocity.X));
        }
    }

    protected override void OnCollisionWithPlayer (
        Collision collision, Player player
    ) {
        if (_isDead) return;

        if (_isShell) {
            if (IsStill) {
                PushShell(collision.HorizontalDirection, collision.Intersection.X);
            }
            else {
                if (IsBeingTrampledByPlayer(player.ColliderPosition)) {
                    StopShell();
                    player.JumpWithStompStrength();
                }
                else {
                    player.TakeDamage(false);
                }
            }
        }
        else {
            if (IsBeingTrampledByPlayer(player.ColliderPosition)) {
                if (_canRevive) {
                    TakeDamage(false);
                    player.JumpWithStompStrength();
                }
                else {
                    Die();
                }

                _sound_stomp.Play();
                player.JumpWithStompStrength();
            }
            else {
                player.TakeDamage(false);
            }
        }
    }

    public override void TakeDamage (
        bool forceDeath, Direction direction = Direction.None
    ) {
        if (forceDeath) {
            DieWithStyle(direction);
        }
        else {
            TurnIntoShell();
        }
    }

    public override void Die () {
        base.Die();
        _animations.SetState((int)AnimStates.DEAD);
        _velocity.X = 0;

        JobManager.AddJob(.25f, Dispose);
    }

    private void TurnIntoShell () {
        _isShell = true;

        _secondsUntilReviveStart = SECONDS_UNTIL_REVIVE_START;
        _velocity.X = 0;
        SetColliderSize(_shellCollider);
        _animations.SetState((int)AnimStates.SHELL);
    }

    private void CheckShellRevive () {
        if (_isShell == false || Velocity.X != 0f) return;

        if (_isReviving) {
            _secondsUntilReviveEnd -= SECONDS_PER_FIXED_UPDATE;

            if (_secondsUntilReviveEnd <= 0f) {
                Revive();
            }
        }
        else {
            _secondsUntilReviveStart -= SECONDS_PER_FIXED_UPDATE;

            if (_secondsUntilReviveStart <= 0f) {
                StartRevive();
            }
        }
    }

    private void StartRevive () {
        _isReviving = true;
        _secondsUntilReviveEnd = SECONDS_UNTIL_REVIVE_END;
        _animations.SetState((int)AnimStates.SHELL_REANIMATING);
    }

    private void Revive () {
        _isShell = false;
        _isReviving = false;

        _velocity.X = _isLookingLeft ? -WALKING_SPEED : WALKING_SPEED;
        SetColliderSize(_defaultCollider);
        _animations.SetState((int)AnimStates.WALKING);
    }

    private void PushShell (Direction direction, float push) {
        const float EXTRA_PUSH = 0.01f;

        if (direction == Direction.Left) {
            Move(-push - EXTRA_PUSH, 0);
            _velocity.X = -SHELL_SPEED;
        }
        else if (direction == Direction.Right) {
            Move(-push + EXTRA_PUSH, 0);
            _velocity.X = SHELL_SPEED;
        }
        else {
            return;
        }

        _isReviving = false;
        _animations.SetState((int)AnimStates.SHELL_TRAVELING);
        _sound_kick.Play();
    }

    private void StopShell () {
        _secondsUntilReviveStart = SECONDS_UNTIL_REVIVE_START;
        _velocity.X = 0f;
        _animations.SetState((int)AnimStates.SHELL);
    }
}
