namespace splatform.physics;
internal class Collision {
    /// <summary>
    /// The collider of the object this one collided with.
    /// </summary>
    public required Collider Collider { get; init; }
    /// <summary>
    /// True if there was a collision.
    /// </summary>
    public required bool IsCollision { get; init; }
    /// <summary>
    /// The side of this collider that collided with the other object
    /// (whichever is greatest between horizontal and vertical directions).
    /// </summary>
    public required Direction Direction { get; init; }
    /// <summary>
    /// The horizontal side of this collider that collided with the other object.
    /// </summary>
    public required Direction HorizontalDirection { get; init; }
    /// <summary>
    /// The vertical side of this collider that collided with the other object.
    /// </summary>
    public required Direction VerticalDirection { get; init; }
    /// <summary>
    /// A vector representing the overlap between the two colliders.
    /// </summary>
    public required vec2 Intersection { get; init; }

    /// <summary>
    /// Returns the side of the game object given that was hit by this collision.
    /// </summary>
    /// <param name="go">The game object to check.</param>
    public Direction GetDirectionFor (IGameObject go) {
        return go == Collider.GameObject ? Direction : Direction.Opposite();
    }

    /// <summary>
    /// Returns the horizontal side of the game object given that was hit by
    /// this collision.
    /// </summary>
    /// <param name="go">The game object to check.</param>
    public Direction GetHorizontalDirectionFor (IGameObject go) {
        return go == Collider.GameObject
            ? HorizontalDirection
            : HorizontalDirection.Opposite();
    }

    /// <summary>
    /// Returns the vertical side of the game object given that was hit by this
    /// collision.
    /// </summary>
    /// <param name="go">The game object to check.</param>
    public Direction GetVerticalDirectionFor (IGameObject go) {
        return go == Collider.GameObject
            ? VerticalDirection 
            : VerticalDirection.Opposite();
    }
}
