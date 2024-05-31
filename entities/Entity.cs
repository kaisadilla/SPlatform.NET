using SFML.Graphics;
using splatform.animation;
using splatform.assets;
using splatform.entities.traits;
using splatform.game.scenes;
using splatform.physics;
using splatform.tiles;

namespace splatform.entities;
internal abstract partial class Entity : IGameObject {
    public const float JUMP_ON_KILL_STRENGTH = 16f * 16f;

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
    public bool DrawBeforeForeground { get; protected set; } = false;

    #region Data members
    protected vec2 _entitySize;
    protected vec2 _textureSize; // TODO: check if this should be ivec2.
    protected IntRect _defaultCollider;
    protected List<Trait> _traits = new();
    #endregion

    #region State members
    /// <summary>
    /// If false, this entity won't do anything on its update() and
    /// fixedUpdate() events.
    /// </summary>
    protected bool _isUpdated = true;
    protected bool _isDead = false;
    protected float _despawnTimer = 0f;
    public bool DisposePending { get; protected set; } = false;

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
    protected AnimationState _animations = new();
    protected Texture _texture;
    protected RectangleShape _sprite = new(); // TODO: Replace with sf::Sprite
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
    public Collider Collider { get; protected set; }
    /// <summary>
    /// If this value is greater than zero, collisions with entities are ignored.
    /// </summary>
    protected float _ignoreEntityCollisionTimer = 0f;
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

    public FloatRect ColliderPosition => Collider.AbsoluteBounds;
    public bool CollidesWithEntities => _isDead == false
        && _ignoreEntityCollisionTimer <= 0f;

    /// <summary>
    /// Equals true when this entity's velocity is below a small threshold.
    /// </summary>
    public bool IsStill => MathF.Abs(_velocity.X) < 0.1f
        && MathF.Abs(_velocity.Y) < 0.1f;
    #endregion

    public vec2 Velocity => _velocity;

    public Entity () {
        Collider = new(this);
    }

    #region Setters
    public void SetPosition (vec2 position) {
        _position = position;
        Collider.SetPosition(position);
        _sprite.Position = PixelPosition + new vec2(0, 1);
    }

    public void SetGridPosition (ivec2 position) {
        SetPosition(new(
            position.X * PIXELS_PER_TILE,
            position.Y * PIXELS_PER_TILE
        ));
    }

    public void SetStartingDirectionRight (bool right) {
        _startingDirectionRight = right;
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

    public void SetColliderSize (IntRect colliderPosition) {
        Collider.CalculateVectorsInsideSprite(
            _size,
            colliderPosition,
            out vec2 colliderCenter,
            out vec2 colliderEdge
        );

        Collider.SetRelativeCenter(colliderCenter);
        Collider.SetDistanceToEdge(colliderEdge);
    }

    public void SetLevel (LevelScene level) {
        _level = level;
    }
    #endregion

    #region Events
    public void Start () {
        OnStart();

        foreach (var trait in _traits) {
            trait.Start();
        }
    }
    public void Update () {
        if (_isUpdated) {
            OnUpdate();

            foreach (var trait in _traits) {
                trait.Update();
            }
        }
        if (_playingAnim) {
            // anim tween stuff
        }
    }

    public void FixedUpdate () {
        if (_isUpdated) {
            OnFixedUpdate();

            foreach (var trait in _traits) {
                trait.FixedUpdate();
            }
        }
    }

    public void LateUpdate () {
        foreach (var trait in _traits) {
            trait.LateUpdate();
        }
    }

    public void Draw (RenderWindow window) {
        window.Draw(_sprite);

        foreach (var trait in _traits) {
            trait.Draw(window);
        }
    }

    public virtual void DrawDebugInfo (RenderWindow window) {
        foreach (var trait in _traits) {
            trait.DrawDebugInfo(window);
        }
    }
    #endregion

    #region Physics
    public virtual void UpdatePhysics (float deltaTime) {
        CheckLookingLeft();
        _velocity.Y = MathF.Min(_velocity.Y + (12f * _gravityScale), 8f * 16f * 4f); // remove magic numbers.

        Move(_velocity * deltaTime);
    }

    public void Move (vec2 direction) {
        SetPosition(new(_position.X + direction.X, _position.Y + direction.Y));
    }

    public void Move (float x, float y) {
        SetPosition(new(_position.X + x, _position.Y + y));
    }

    public void CheckCollisionWithTiles (List<Tile> tiles, int startingIndex = 0) {
        const float COLLISION_THRESHOLD = 1.5f;

        if (_ignoresTiles) return;

        _isGrounded = false;

        List<int> secondRound = new();

        for (int i = startingIndex; i < tiles.Count; i++) {
            var tile = tiles[i];

            if (tile.Collider == null) continue;

            if (Collider.CheckCollision(tile.Collider, out var collision) == false) {
                continue;
            }
           
            if (IsCollisionValid(collision, tile) == false) continue;

            if (collision.Direction == Direction.Up || collision.Direction == Direction.Down) {
                _collided = true;

                if (_canGoThroughTiles == false) {
                    if (MathF.Abs(collision.Intersection.X) > COLLISION_THRESHOLD) {
                        Move(0, collision.Intersection.Y);
                        _velocity.Y = 0f;

                        if (collision.Direction == Direction.Down) {
                            _isGrounded = true;
                        }
                    }
                }

                OnCollisionWithTile(collision, tile);
            }
            else if (collision.Direction == Direction.Left || collision.Direction == Direction.Right) {
                secondRound.Add(i);
            }
        }

        foreach (int i in secondRound) {
            var tile = tiles[i];

            if (tile.Collider == null) continue;
            
            if (Collider.CheckCollision(tile.Collider, out Collision collision)) {
                if (IsCollisionValid(collision, tile) == false) continue;

                _collided = true;

                if (_canGoThroughTiles == false) {
                    _velocity.X = 0f;

                    // Prevent unexpected hits from the side by checking its
                    // vertical component is above a certain threshold.
                    if (MathF.Abs(collision.Intersection.Y) > COLLISION_THRESHOLD) {
                        Move(collision.Intersection.X, 0);
                    }
                }

                OnCollisionWithTile(collision, tile);
            }
        }
    }

    public void CheckCollisionWithEntities (List<Entity> entities, int startingIndex = 0) {
        if (CollidesWithEntities == false) return;

        for (int i = startingIndex; i < entities.Count; i++) {
            var entity = entities[i];

            if (entity.CollidesWithEntities == false) continue;

            if (Collider.CheckCollision(entity.Collider, out var collision)) {
                this.TriggerCollisionWithEntityEvent(collision, entity);
                entity.TriggerCollisionWithEntityEvent(collision, this);
            }
        }
    }

    public bool IsCollisionValid (Collision collision, Tile tile) {
        return tile.HasMobCollided(collision, _velocity);
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
        _velocity.Y = -strength;
    }

    public abstract void TakeDamage (
        bool forceDeath, Direction direction = Direction.None
    );

    public virtual void Die () {
        _isDead = true;
    }

    public virtual void Dispose () {
        DisposePending = true;
    }
    #endregion

    protected virtual void OnStart () {

    }

    protected virtual void OnUpdate () {
        // TODO: This should be in Update
        _animations.OnUpdate(Time.DeltaTime, _animationSpeed);

        var uvs = _animations.CurrentAnimation.GetCurrentSlice(
            _isLookingLeft && _flipSpriteWhenLookingLeft
        );

        if (_flipHorizontal) {
            uvs.Left += uvs.Width;
            uvs.Width = -uvs.Width;
        }
        if (_flipVertical) {
            uvs.Top += uvs.Height;
            uvs.Height = -uvs.Height;
        }

        _sprite.TextureRect = uvs;

        if (_isDead) {
            _despawnTimer -= Time.DeltaTime;
            if (_despawnTimer < 0f) {
                Dispose();
            }
        }
    }

    protected virtual void OnFixedUpdate () {
        if (_ignoreEntityCollisionTimer > 0f) {
            _ignoreEntityCollisionTimer = MathF.Max(
                0,
                _ignoreEntityCollisionTimer - SECONDS_PER_FIXED_UPDATE
            );
        }

        // TODO: This should be in FixedUpdate
        UpdatePhysics(SECONDS_PER_FIXED_UPDATE);
        CheckOutOfBounds();
    }

    protected virtual void OnCollisionWithTile (Collision collision, Tile tile) {
        // Nothing.
    }

    protected virtual void OnCollisionWithItem (Collision collision, Item item) {
        // Nothing.
    }

    protected virtual void OnCollisionWithEnemy (Collision collision, Enemy enemy) {
        // Nothing.
    }

    protected virtual void OnCollisionWithPlayer (Collision collision, Player player) {
        // Nothing.
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

    #region Helper methods
    /// <summary>
    /// Sets this entity's horizontal speed to the one given, either to the
    /// left or to the right.
    /// </summary>
    /// <param name="toTheRight">True if it'll move to the right, false if
    /// it'll move to the left.</param>
    /// <param name="speed">Its initial speed.</param>
    protected void StartMovement (bool toTheRight, float speed) {
        _velocity.X = toTheRight ? speed : -speed;
    }

    /// <summary>
    /// Changes the horizontal velocity of this enemy so it walks away from
    /// a tile it has collided.
    /// </summary>
    /// <param name="collision">The collision with the tile.</param>
    /// <param name="walkingSpeed">This enemy's walking speed.</param>
    protected void WalkAwayFromTile (Collision collision, float walkingSpeed) {
        // the tile was to the left of this entity.
        if (collision.Direction == Direction.Left) {
            _velocity.X = walkingSpeed;
        }
        else if (collision.Direction == Direction.Right) {
            _velocity.X = -walkingSpeed;
        }
    }

    /// <summary>
    /// Changes the horizontal velocity of this enemy so it walks away from
    /// an enemy it has collided.
    /// </summary>
    /// <param name="collision">The collision with the enemy.</param>
    /// <param name="walkingSpeed">This enemy's walking speed.</param>
    protected void WalkAwayFromEntity (Collision collision, float walkingSpeed) {
        // this entity was to the left of the other entity.
        if (collision.GetDirectionFor(this) == Direction.Left) {
            _velocity.X = -walkingSpeed;
        }
        else if (collision.GetDirectionFor(this) == Direction.Right) {
            _velocity.X = walkingSpeed;
        }
    }
    #endregion

    /// <summary>
    /// Triggers the onCollision evenet appropriate for the type of entity given,
    /// verifying first that the collision should be triggered for that type of
    /// entity.
    /// </summary>
    /// <param name="collision">The collision that triggered this event.</param>
    /// <param name="entity">The entity this one collided with.</param>
    private void TriggerCollisionWithEntityEvent (
        Collision collision, Entity entity
    ) {
        if (entity.Type == GameObjectType.Enemy) {
            if (this._ignoresMobs == false && entity._ignoresMobs == false) {
                OnCollisionWithEnemy(collision, (Enemy)entity);
            }
        }
        else if (entity.Type == GameObjectType.Item) {
            OnCollisionWithItem(collision, (Item)entity);
        }
        else if (entity.Type == GameObjectType.Player) {
            OnCollisionWithPlayer(collision, (Player)entity);
        }
    }

    public void __TEMP_set_sprite_by_filename (string name, vec2 size) {
        _texture = new(PATH_ENTITY_SPRITES + "/" + name + ".png");

        _sprite = new RectangleShape(size) {
            Texture = _texture,
            TextureRect = new(0, 0, (int)size.X, (int)size.Y),
        };
    }
}
