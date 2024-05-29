using SFML.Graphics;
using splatform.animation;
using splatform.assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.tiles;
internal partial class Tile {
    //private vec2 _position;
    //private vec2 _drawPosition;
    //
    ///// <summary>
    ///// The position of the tile inside its level.
    ///// </summary>
    //public required vec2 Position {
    //    get => _position;
    //    init {
    //        _position = value;
    //        DrawPosition = value;
    //    }
    //}
    ///// <summary>
    ///// The point in the level where the tile is drawn, which may or may not
    ///// correspond to its actual position.
    ///// </summary>
    //public required vec2 DrawPosition {
    //    get => _drawPosition;
    //    init {
    //        _drawPosition = value;
    //        RecalculateSpritePosition();
    //    }
    //}

    private AnimationState _animations = new();

    public vec2 Position { get; private set; }
    public vec2 DrawPosition { get; private set; }

    public Sprite Sprite { get; private set; } = new();

    public Tile () {
        Sprite.Texture = Assets.TileAtlas;
    }

    public virtual void OnStart () {
        Sprite.TextureRect = _animations.CurrentAnimation.GetCurrentSlice(false);
    }

    public virtual void OnUpdate () {
        _animations.OnUpdate(Time.DeltaTime, Time.TimeScale);
        Sprite.TextureRect = _animations.CurrentAnimation.GetCurrentSlice(false);
    }

    /// <summary>
    /// Draws this tile in the window given.
    /// </summary>
    /// <param name="window">The window in which to draw the tile.</param>
    public void Draw (RenderWindow window) {
        window.Draw(Sprite);
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

    public void SetDrawPosition (vec2 position) {
        DrawPosition = position;
        RecalculateSpritePosition();
    }

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
