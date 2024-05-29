﻿using SFML.Graphics;
using splatform.animation;
using splatform.assets;
using splatform.game.scenes;
using splatform.physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace splatform.entities;
internal abstract partial class Entity : IGameObject {
    public abstract GameObjectType Type { get; }

    protected vec2 _size;
    protected vec2 _position;

    /// <summary>
    /// True if this entity starts walking to the right, false if it walks to
    /// the left.
    /// </summary>
    protected bool _startingDirectionRight = false; // TODO: Move to enemy class?
    /// <summary>
    /// If true, this entity is drawn before the foreground layer, which means
    /// this entity will be covered by tiles in this layer.
    /// </summary>
    protected bool _drawBeforeForeground = false;

    #region Data members
    protected vec2 _entitySize;
    protected vec2 _textureSize; // TODO: check if this should be ivec2.
    protected IntRect _defaultCollider;
    #endregion

    #region State members
    /// <summary>
    /// If false, this entity won't do anything on its update() and
    /// fixedUpdate() events.
    /// </summary>
    protected bool _isUpdated = true;
    protected bool _isDead = false;
    protected float _despawnTimer = 0f;
    protected bool _disposePending = false;

    protected bool _collided = false;
    protected bool _isGrounded = false;
    protected bool _isLookingLeft = false;
    #endregion

    #region Movement and interaction members
    /// <summary>
    /// The level scene this entity is in.
    /// </summary>
    protected LevelScene _level;
    /// <summary>
    /// If true, this entity won't be pushed out of solid tiles when it
    /// collides with them. If the entity has gravity, this means it will fall
    /// through the floor.
    /// </summary>
    protected bool _canGoThroughTiles = false;
    /// <summary>
    /// If true, this entity doesn't check collisions with tiles.
    /// </summary>
    protected bool _ignoresTiles = false;
    /// <summary>
    /// If true, this entity doesn't check collisions with mobs.
    /// </summary>
    protected bool _ignoresMobs = false;
    /// <summary>
    /// If true, this entity will be destroyed if it leaves its level's bounds.
    /// </summary>
    protected bool _destroyWhenOutOfBounds = true;
    /// <summary>
    /// If true, this entity's sprite will be mirrored when it's looking to the
    /// left.
    /// </summary>
    protected bool _flipSpriteWhenLookingLeft = true;
    #endregion

    #region Physics members
    protected vec2 _velocity = new(0, 0);
    protected float _gravityScale = 1f;
    #endregion

    #region Sprite members
    protected AnimationState _animations;
    protected Texture _texture;
    protected RectangleShape _sprite; // TODO: Replace with sf::Sprite
    protected float _animationSpeed = 1f;
    /// <summary>
    /// If true, this entity's sprite will be horizontally flipped.
    /// </summary>
    protected bool _flipHorizontal = false;
    /// <summary>
    /// If true, this entity's sprite will be vertically flipped.
    /// </summary>
    protected bool _flipVertical = false;
    /// <summary>
    /// True while this entity is playing a transitional animation.
    /// </summary>
    protected bool _playingTransitionalAnim = false;
    /// <summary>
    /// An animation that overrides this entity's normal animation(s).
    /// This is used, for example, when Mario picks up a mushroom and displays
    /// a "growing" animation before he becomes Big Mario.
    /// </summary>
    protected DynamicAnimation? _transitionalAnim = null;
    #endregion

    #region Collider members
    // TODO: Physics
    #endregion

    #region Animation members
    protected bool _playingAnim = false;
    // TODO: protected TweenAnimation<float> _anim_getFromBlock;
    #endregion

    public int Id { get; protected set; } = -1;

    #region Helper members
    /// <summary>
    /// This entity's position aligned with the pixel grid.
    /// </summary>
    public vec2 PixelPosition => new(
        MathF.Floor(_position.X),
        MathF.Floor(_position.Y)
    );

    /// <summary>
    /// This entity's position within the level's tile grid.
    /// </summary>
    public ivec2 GridPosition => new(
        (int)MathF.Floor(_position.X / 16f),
        (int)MathF.Floor(_position.Y / 16f)
    );
    #endregion

    public Entity () {
        // colider = new(this);
    }

    #region Setters
    public void SetPosition (vec2 position) {
        _position = position;
        //collider.setPosition(position);
        _sprite.Position = PixelPosition + new vec2(0, 1);
    }
    #endregion

    #region Initialization
    public void SetDefaultSizes (
        vec2 entitySize, vec2 textureSize, IntRect collider
    ) {
        _entitySize = entitySize;
        _textureSize = textureSize;
        _defaultCollider = collider;

        _size = entitySize;
        _sprite.Size = _entitySize;

        SetColliderSize(_defaultCollider);
    }

    public void SetSprite (int spriteIndex) {
        string texName = Assets.GetEntitySpriteAt(spriteIndex);

        _texture = new(PATH_ENTITY_SPRITES + "/" + texName + ".png");
        _sprite = new(_entitySize) {
            Texture = _texture,
            TextureRect = new(0, 0, (int)_textureSize.X, (int)_textureSize.Y)
        };
    }

    public void SetColliderSize (IntRect collider) {
        // TODO: Implement.
    }

    public void SetLevel (LevelScene level) {
        _level = level;
    }
    #endregion

    #region Events
    public void Start () {
        OnStart();
    }
    public void Update () {
        if (_isUpdated) {
            OnUpdate();
        }
        if (_playingAnim) {
            // anim tween stuff
        }
    }

    public void FixedUpdate () {
        if (_isUpdated) {
            OnFixedUpdate();
        }
    }
    #endregion

    #region Physics
    public virtual void UpdatePhysics (float deltaTime) {
        CheckLookingLeft();
        _velocity.Y = MathF.Min(_velocity.Y + (12f * _gravityScale), 8f * 16f * 4f); // remove magic numbers.

        Move(_velocity * deltaTime);
    }

    public virtual void Move (vec2 direction) {
        SetPosition(new(_position.X + direction.X, _position.Y + direction.Y));
    }

    public virtual void Move (float x, float y) {
        SetPosition(new(_position.X + x, _position.Y + y));
    }
    #endregion

    #region Animations
    public void PlayAnim_GetFromBlock () {
        // todo
    }
    #endregion

    /// <summary>
    /// Changes the sizes of this entity's sprite and collider. The values
    /// given are set up as its default values.
    /// </summary>
    /// <param name="spriteSize">The size of the sprite, in pixels.</param>
    /// <param name="colliderPosition">The position of the collider relative to
    /// the sprite.</param>
    public void InitializeDefaultSpriteAndColliderSizes ( // TODO: REMOVE, THIS IS JUST TEMPORARY
        vec2 spriteSize, IntRect colliderPosition
    ) {
        _size = spriteSize;
        _defaultCollider = colliderPosition;

        _sprite.Size = _size;
        SetColliderSize(_defaultCollider);
    }

    #region Actions
    public void Jump (float strength) {
        _velocity.Y -= strength;
    }

    public void Die () {
        _isDead = true;
    }

    public virtual void Dispose () {
        _disposePending = true;
    }
    #endregion

    protected virtual void OnStart () {

    }

    protected virtual void OnUpdate () {

    }

    protected virtual void OnFixedUpdate () {
        // if (ignoreEntityCollisionTimer > 0.f)

        UpdatePhysics(SECONDS_PER_FIXED_UPDATE);
        CheckOutOfBounds();
    }

    protected virtual void CheckOutOfBounds () {
        if (_position.X < -32f || _position.X > _level.PixelWidth + 32f) {
            Dispose();
        }
        else if (_position.Y < -240f || _position.Y > _level.PixelWidth + 32f) {
            Dispose();
        }
    }

    /// <summary>
    /// Determines whether this entity is looking left or right and updates the
    /// field <see cref="_isLookingLeft"/> accordingly.
    /// </summary>
    protected virtual void CheckLookingLeft () {
        // Note: when horizontal speed is exactly 0, we don't update this value.
        if (_velocity.X < 0f) {
            _isLookingLeft = true;
        }
        else if (_velocity.X > 0f) {
            _isLookingLeft = false;
        }
    }

    protected virtual void DrawDebugInfo (RenderWindow window) { }
}