using SFML.Graphics;

namespace splatform.physics;
internal class Collider {
    public IGameObject GameObject { get; private set; }

    private vec2 _position;
    private vec2 _relativeCenter;
    private vec2 _distanceToEdge;
    private bool _collidedThisFrame = false;

    public vec2 AbsoluteCenter => _position + _relativeCenter;
    public FloatRect RelativeBounds => new(
        _relativeCenter - _distanceToEdge,
        _distanceToEdge * 2f
    );
    public FloatRect AbsoluteBounds => new(
        AbsoluteCenter - _distanceToEdge,
        _distanceToEdge * 2f
    );

    public Collider (IGameObject gameObject) {
        GameObject = gameObject;
    }

    public Collider (
        IGameObject gameObject,
        vec2 position,
        vec2 relativeCenter,
        vec2 distanceToEdge
    ) {
        GameObject = gameObject;
        _position = position;
        _relativeCenter = relativeCenter;
        _distanceToEdge = distanceToEdge;
    }

    /// <summary>
    /// Calculates the center and distance to the edge of a collider relative
    /// to a sprite, given the dimensions of the sprite and a rectangle that
    /// represents the position of the collider inside it.
    /// </summary>
    /// <param name="spriteSize">The dimensions of the sprite.</param>
    /// <param name="colliderPosition">A rectangle representing the collider
    /// inside the sprite, with the (x, y) coordinates of the top left corner
    /// and the width and height of the collider.</param>
    /// <param name="relativeCenter">Returns the center obtained by the
    /// calculation.</param>
    /// <param name="distanceToEdge">Returns the distance to the edge obtained
    /// by the calculation.</param>
    public static void CalculateVectorsInsideSprite (
        vec2 spriteSize,
        IntRect colliderPosition,
        out vec2 relativeCenter,
        out vec2 distanceToEdge
    ) {
        // The offsets of the collider's top-left corner in relation to the
        // sprite's top-left corner.
        int xOffset = colliderPosition.Left;
        int yOffset = colliderPosition.Top;

        // The center of the collider relative to itself, which is also the
        // distance to the edge.
        float xHalf = colliderPosition.Width / 2f;
        float yHalf = colliderPosition.Height / 2f;

        // Adding the top-left offset to the center returns the position of the
        // center relative to the sprite.
        relativeCenter = new(xHalf + xOffset, yHalf + yOffset);
        distanceToEdge = new(xHalf, yHalf);
    }

    #region Setters
    public void SetPosition (vec2 position) {
        _position = position;
    }

    public void SetRelativeCenter (vec2 relativeCenter) {
        _relativeCenter = relativeCenter;
    }

    public void SetDistanceToEdge (vec2 distanceToEdge) {
        _distanceToEdge = distanceToEdge;
    }
    #endregion

    public bool CheckCollision (Collider collider, out Collision collision) {
        // The distances of the centers of the two items.
        float xDelta = AbsoluteCenter.X - collider.AbsoluteCenter.X;
        float yDelta = AbsoluteCenter.Y - collider.AbsoluteCenter.Y;

        // Check if the sum of both colliders' distances to their respective
        // edges is higher than the distance between their centers.
        float xIntersect = MathF.Abs(xDelta)
            - (_distanceToEdge.X + collider._distanceToEdge.X);
        float yIntersect = MathF.Abs(yDelta)
            - (_distanceToEdge.Y + collider._distanceToEdge.Y);

        bool wasCollision = xIntersect < 0f && yIntersect < 0f;

        var mainDir = Direction.None;
        var horizDir = Direction.None;
        var vertDir = Direction.None;

        if (wasCollision) {
            horizDir = xDelta > 0f ? Direction.Left : Direction.Right;
            vertDir = yDelta > 0f ? Direction.Up : Direction.Down;

            mainDir = xIntersect > yIntersect ? horizDir : vertDir;
        }

        // Remember that, when a collision occurs, x and y intersect will
        // always be negative, so -xIntersect results in a positive number.
        float xSignedIntersect = xDelta > 0f ? -xIntersect : xIntersect;
        float ySignedIntersect = yDelta > 0f ? -yIntersect : yIntersect;

        collision = new() {
            Collider = collider,
            IsCollision = wasCollision,
            Direction = mainDir,
            HorizontalDirection = horizDir,
            VerticalDirection = vertDir,
            Intersection = new(xSignedIntersect, ySignedIntersect),
        };

        _collidedThisFrame = wasCollision;
        return wasCollision;
    }

    public void DrawColliderBounds (RenderWindow window) {
        DrawColliderBounds(window, _collidedThisFrame ? Color.Red : Color.Blue);
    }

    public void DrawColliderBounds (RenderWindow window, Color color) {
        const float OFFSET = 0.1f;

        Vertex[] lines = new Vertex[5];

        lines[0].Position = AbsoluteCenter - _distanceToEdge + new vec2(OFFSET, OFFSET);
        lines[1].Position = new vec2(
            AbsoluteCenter.X - _distanceToEdge.X + OFFSET,
            AbsoluteCenter.Y + _distanceToEdge.Y
        );
        lines[2].Position = AbsoluteCenter + _distanceToEdge;
        lines[3].Position = new vec2(
            AbsoluteCenter.X + _distanceToEdge.X,
            AbsoluteCenter.Y - _distanceToEdge.Y + OFFSET
        );
        lines[4].Position = AbsoluteCenter - _distanceToEdge + new vec2(OFFSET, OFFSET);

        lines[0].Color = color;
        lines[1].Color = color;
        lines[2].Color = color;
        lines[3].Color = color;
        lines[4].Color = color;

        window.Draw(lines, PrimitiveType.LineStrip);
    }
}
