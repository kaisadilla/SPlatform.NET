using SFML.Audio;
using SFML.Window;
using splatform.animation;
using splatform.assets;
using splatform.physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.entities;
internal class Player : Entity {
    public override GameObjectType Type => GameObjectType.Player;

    const float ACCELERATION_X = 16f * 20f;
    const float MAX_SPEED_X = 16f * 5.5f;
    /// <summary>
    /// The window of time, in seconds, that a jump instruction is "saved" if
    /// it cannot be executed immediately.
    /// </summary>
    const float JUMP_BUFFER = 0.1f;

    public enum MarioMode {
        Small,
        Big,
        Fire,
        Raccoon,
        Frog,
        Tanooki,
        Hammer,
    }

    public enum AnimState {
        SmallStill,
        SmallWalking,
        SmallSkid,
        SmallJumping,
        Dead,
        BigStill,
        BigWalking,
        BigSkid,
        BigJumping,
        BigCrouching,
        TransformSmallToBig,
    }

    private MarioMode _currentMode = MarioMode.Small;

    #region Jump params
    private float _jumpStart = 0f;
    private float _jumpTime = 0f;
    private float _jumpMinTime = 0f;
    private float _jumpMaxTime = 0f;
    private bool _isJumping = false;
    private bool _jumpReleased = true;
    private float _jumpBufferTime = 0f;
    #endregion

    #region Sounds
    private Sound _sound_jump;
    private Sound _sound_death;
    #endregion

    #region Helpers
    private AnimState CurrentAnimState_Still => _currentMode switch {
        MarioMode.Small => AnimState.SmallStill,
        MarioMode.Big => AnimState.BigStill,
        _ => AnimState.SmallStill,
    };
    private AnimState CurrentAnimState_Walking => _currentMode switch {
        MarioMode.Small => AnimState.SmallWalking,
        MarioMode.Big => AnimState.BigWalking,
        _ => AnimState.SmallWalking,
    };
    private AnimState CurrentAnimState_Skid => _currentMode switch {
        MarioMode.Small => AnimState.SmallSkid,
        MarioMode.Big => AnimState.BigSkid,
        _ => AnimState.SmallSkid,
    };
    private AnimState CurrentAnimState_Jumping => _currentMode switch {
        MarioMode.Small => AnimState.SmallJumping,
        MarioMode.Big => AnimState.BigJumping,
        _ => AnimState.SmallJumping,
    };
    private AnimState CurrentAnimState_Crouching => _currentMode switch {
        MarioMode.Small => AnimState.SmallStill,
        MarioMode.Big => AnimState.BigCrouching,
        _ => AnimState.SmallStill,
    };
    #endregion

    public Player () {
        _destroyWhenOutOfBounds = false;
        _sound_jump = new(Assets.Sounds!.Jump);
        _sound_death = new(Assets.Sounds!.PlayerDeath);
        Collider = new(this);
    }

    public void __TEMP_initialize_animations () {
        ivec2 slices = new(16, 8);
        // hardcoded values, will be replaced by dynamically loaded ones later.
        var aSmallStill = new StaticAnimation(slices, _textureSize, 0);
        var aSmallWalking = new DynamicAnimation(slices, _textureSize, 0.1f, [0, 1]);
        var aSmallSkidding = new StaticAnimation(slices, _textureSize, 3);
        var aSmallJumping = new StaticAnimation(slices, _textureSize, 2);
        var aDead = new StaticAnimation(slices, _textureSize, 4);
        var aBigStill = new StaticAnimation(slices, _textureSize, 16);
        var aBigWalking = new DynamicAnimation(slices, _textureSize, 0.075f, [16, 17, 18, 17]);
        var aBigSkidding = new StaticAnimation(slices, _textureSize, 20);
        var aBigJumping = new StaticAnimation(slices, _textureSize, 19);
        var aBigCrouching = new StaticAnimation(slices, _textureSize, 21);

        // TODO: idk what I'll do with this.
        DynamicAnimation aTransformSmallToBig = new(
            slices,
            _textureSize,
            0.0666f,
            [22, 0, 22, 0, 22, 0, 22, 16, 22, 16, 22, 16],
            () => Console.WriteLine("Nothing happens here yet")
        );

        _animations.AddAnimation(aSmallStill);
        _animations.AddAnimation(aSmallWalking);
        _animations.AddAnimation(aSmallSkidding);
        _animations.AddAnimation(aSmallJumping);
        _animations.AddAnimation(aDead);
        _animations.AddAnimation(aBigStill);
        _animations.AddAnimation(aBigWalking);
        _animations.AddAnimation(aBigSkidding);
        _animations.AddAnimation(aBigJumping);
        _animations.AddAnimation(aBigCrouching);
    }

    #region Interactions
    public void SetMode (MarioMode mode) {
        if (mode == MarioMode.Big) {
            _currentMode = mode;
        }
        UpdateColliderDimensions();
    }

    public void TakeDamage (bool forceDeath, Direction direction) {
        Die();
    }

    public void EarnCoin () {
        // TODO
    }
    #endregion

    #region Events
    protected override void OnStart () {
        base.OnStart();
    }

    protected override void OnUpdate () {
        base.OnUpdate();
        ProcessInput();
        SetAnimationState();
    }

    protected override void OnFixedUpdate () {
        base.OnFixedUpdate();
        CheckLevelBoundaries();
    }
    #endregion

    #region Input
    private void ProcessInput () {
        if (_jumpBufferTime > 0f) {
            _jumpBufferTime -= Time.DeltaTime;
        }

        float acc = ACCELERATION_X * Time.DeltaTime;
        float maxSpeed = MAX_SPEED_X;

        // sprinting
        if (Keyboard.IsKeyPressed(Keyboard.Key.C)) {
            maxSpeed *= 1.555f; // Original goes from 18 to 28, which is ~1.555...
        }
        // almost freeze for debug
        if (Keyboard.IsKeyPressed(Keyboard.Key.LControl)) {
            // maxSpeed = 2f;
        }
        if (_isGrounded == false) {
            acc *= 0.75f;
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) {
            if (_velocity.X > 0f) {
                acc *= 2f;
            }

            _velocity.X = MathF.Max(-maxSpeed, _velocity.X - acc);
        }
        else if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) {
            if (_velocity.X < 0f) {
                acc *= 2f;
            }

            _velocity.X = MathF.Min(-maxSpeed, _velocity.X - acc);
        }
        else {
            if (_isGrounded) {
                // Start losing speed if the player is moving, or stop
                // completely when below 0.1 abs horizontal speed.
                if (_velocity.X < 0.1f) {
                    _velocity.X = MathF.Min(
                        0f,
                        _velocity.X + (ACCELERATION_X * Time.DeltaTime)
                    );
                }
                else if (_velocity.X > 0.1f) {
                    _velocity.X = MathF.Max(
                        0f,
                        _velocity.X - (ACCELERATION_X * Time.DeltaTime)
                    );
                }
                else {
                    _velocity.X = 0f;
                }
            }
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.X)) {
            if (_isJumping) {
                PlayerJump();
            }
            // Queue up a jump command, whether or not the player can jump now.
            else if (_isJumping == false && _jumpReleased) {
                _jumpBufferTime = JUMP_BUFFER;
            }

            // If there's a jump command queued up and the player can jump, do so.
            if (_jumpBufferTime > 0f && _isGrounded) {
                PlayerJumpStart();
            }

            _jumpReleased = false;
        }
        else if (_isJumping) {
            PlayerJumpEnd(false);
        }
        else {
            _jumpReleased = true;
        }
    }

    public void PlayerJumpStart () {
        _jumpStart = Time.CurrentTime;
        _jumpTime = 0f;
        _jumpMinTime = 0.08f;
        _jumpMaxTime = 0.4f;
        _isJumping = true;

        _sound_jump.Play();

        _gravityScale = 0f;
        PlayerJump();
    }

    public void PlayerJump () {
        if (_isJumping) {
            _jumpTime += Time.DeltaTime;
            if (_jumpTime > _jumpMaxTime) {
                PlayerJumpEnd(false);
            }
            else {
                _velocity.Y = (-16f * 4f) / 0.4f;
            }
        }
    }

    public void PlayerJumpEnd (bool forced) {
        if (_isJumping) {
            if (forced == false && _jumpTime < _jumpMinTime) {
                PlayerJump();
            }
            else {
                _gravityScale = 1f;
                _isJumping = false;
                _jumpTime = 0f;
            }
        }
    }
    #endregion

    #region Overrides
    protected override void CheckLookingLeft () {
        base.CheckLookingLeft();

        // TODO: This could probably be simplified as "the player is always
        // looking in the direction he's pressing".

        if (_isGrounded) {
            // When skidding, the player is looking at the direction he's trying to move in.
            if (_velocity.X < 0f && Keyboard.IsKeyPressed(Keyboard.Key.Right)) {
                _isLookingLeft = false;
            }
            else if (_velocity.X > 0f && Keyboard.IsKeyPressed(Keyboard.Key.Left)) {
                _isLookingLeft = true;
            }
        }
        // when airborne, the player is looking at the direction he's trying
        // to look at.
        else {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) {
                _isLookingLeft = true;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) {
                _isLookingLeft = false;
            }
        }
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Keeps the player within the boundaries of the level.
    /// </summary>
    private void CheckLevelBoundaries () {
        if (_position.X < 0f) {
            _position.X = 0f;
        }
        else if (_position.X > _level.PixelWidth - _size.Y) {
            _position.X = _level.PixelWidth - _size.Y;
        }
    }

    private void SetAnimationState () {
        _animationSpeed = 1f;
        if (_isDead) {
            _animations.SetState((int)AnimState.Dead);
            return;
        }

        if (_isGrounded) {
            if (_velocity.X == 0f) {
                _animations.SetState((int)CurrentAnimState_Still);
            }
            else if (_velocity.X < 0f) {
                // if the player is moving left but pressing right, they are skidding.
                if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) {
                    _animations.SetState((int)CurrentAnimState_Skid);
                }
                else {
                    _animationSpeed = MathF.Abs(_velocity.X / 64f);
                    _animations.SetState((int)CurrentAnimState_Walking);
                }
            }
            else if (_velocity.X > 0f) {
                // if the player is moving right but pressing left, they are skidding.
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) {
                    _animations.SetState((int)CurrentAnimState_Skid);
                }
                else {
                    _animationSpeed = MathF.Abs(_velocity.X / 64f);
                    _animations.SetState((int)CurrentAnimState_Walking);
                }
            }
            return;
        }

        // not dead, not grounded
        _animations.SetState((int)CurrentAnimState_Jumping);
    }

    private void UpdateColliderDimensions () {
        // TODO: Parameterize
        if (_currentMode == MarioMode.Small) {
            SetColliderSize(new(11, 18, 10, 14));
        }
        else if (_currentMode == MarioMode.Big) {
            SetColliderSize(new(10, 7, 12, 25));
        }
    }
    #endregion
}
