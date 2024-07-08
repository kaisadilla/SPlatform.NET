using SFML.Graphics;
using splatform.animation;
using splatform.assets;
using splatform.entities;
using splatform.game.scenes;
using splatform.physics;

namespace splatform.tiles;
internal partial class OldTile : IGameObject {
    public GameObjectType Type => GameObjectType.Tile;

    public vec2 Position { get; private set; }
    public vec2 DrawPosition { get; private set; }

    public Sprite Sprite { get; private set; } = new();

    public Collider? Collider { get; private set; }

    protected AnimationState _animations = new();
    protected LevelScene? _level;

    public OldTile () {
        Sprite.Texture = Assets.TileAtlas;
    }

    #region Setters
    public void SetLevel (LevelScene level) {
        _level = level;
    }

    /// <summary>
    /// Sets the position of this tile in the PIXEL grid. Note that, to
    /// position this tile in its tile grid position, you should use
    /// <see cref="SetGridPosition(vec2)"/> instead.
    /// </summary>
    /// <param name="position">A position on the PIXEL grid.</param>
    /// <param name="updateDrawPosition">If true, the draw position of this
    /// tile will also be updated to its position in the grid.</param>
    public void SetPosition (vec2 position, bool updateDrawPosition = true) {
        Position = position;
        if (updateDrawPosition) {
            SetDrawPosition(position);
        }
    }

    /// <summary>
    /// Sets the position of this tile in the TILE grid.
    /// </summary>
    /// <param name="position">A position on the TILE grid.</param>
    /// <param name="updateDrawPosition">If true, the draw position of this
    /// tile will also be updated to its position in the grid.</param>
    public void SetGridPosition (vec2 position, bool updateDrawPosition = true) {
        SetPosition(
            new(
                position.X * PIXELS_PER_TILE,
                position.Y * PIXELS_PER_TILE
            ),
            updateDrawPosition
        );
    }

    /// <summary>
    /// Moves this tile's display position by the vector given.
    /// </summary>
    /// <param name="vec">The vector describing the movement.</param>
    public void SetDrawPosition (vec2 vec) {
        DrawPosition = vec;
        RecalculateSpritePosition();
    }

    /// <summary>
    /// Moves this tile's display position by the vector given.
    /// </summary>
    public void MoveDrawPosition (int x, int y) {
        SetDrawPosition(DrawPosition + new vec2(x, y));
    }

    public void MoveDrawPosition (vec2 vec) {
        SetDrawPosition(DrawPosition + vec);
    }

    public void RestartDrawPosition () {
        SetDrawPosition(Position);
    }
    #endregion

    /// <summary>
    /// Draws this tile in the window given.
    /// </summary>
    /// <param name="window">The window in which to draw the tile.</param>
    public void Draw (RenderWindow window) {
        window.Draw(Sprite);
    }

    #region Events
    // TODO: Split Update() and OnUpdate(), and so on for all methods.
    public virtual void OnStart () {
        Sprite.TextureRect = _animations.CurrentAnimation.GetCurrentSlice(false);
    }

    public virtual void OnUpdate () {
        _animations.OnUpdate(Time.DeltaTime, Time.TimeScale);
        Sprite.TextureRect = _animations.CurrentAnimation.GetCurrentSlice(false);
    }

    public virtual bool HasMobCollided (Collision collision, vec2 mobVelocity) {
        return true; // TODO
    }
    /// <summary>
    /// An event that must be called manually by the Player class when necessary.
    /// </summary>
    /// <param name="collision">The collision that triggered this effect.</param>
    /// <param name="player">The player that triggered this effect.</param>
    public virtual void OnCollisionWithPlayer (Collision collision, Player player) {
        // Nothing.
    }
    #endregion


    /// <summary>
    /// Moves the actual Sprite of this tile to the position in the pixel
    /// grid represented by the DrawPosition property.
    /// </summary>
    private void RecalculateSpritePosition () {
        Sprite.Position = new(
            MathF.Floor(DrawPosition.X),
            MathF.Floor(DrawPosition.Y)
        );
    }
}
