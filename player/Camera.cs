using SFML.Graphics;

namespace splatform.player;
internal class Camera {
    /// <summary>
    /// The width and height of the window, in pixels.
    /// </summary>
    private ivec2 _windowSize;
    /// <summary>
    /// Half the width and height of the window, in pixels.
    /// </summary>
    private ivec2 _halfWindowSize;
    private vec2 _viewMins;
    private vec2 _viewMaxes;

    public View View { get; private set ; }

    public vec2 TopLeft => View.Center - (vec2)_halfWindowSize;

    public Camera (ivec2 levelDimensions, ivec2 windowDimensions) {
        _windowSize = windowDimensions;
        _halfWindowSize = new(_windowSize.X / 2, _windowSize.Y / 2);
        View = new View(new FloatRect(0, 0, _windowSize.X, _windowSize.Y));

        int xMin = _halfWindowSize.X;
        int yMin = _halfWindowSize.Y;
        int xMax = levelDimensions.X - _halfWindowSize.X;
        int yMax = levelDimensions.Y - _halfWindowSize.Y;

        // for y, if the min is higher than the max, it becomes the max.
        // This means that we'll always show more sky instead of more underground.
        yMin = Math.Min(yMin, yMax);
        // for x, if the max is lower than the min, it becomes the min.
        // This means that we'll always show leftover space to the right
        // instead of the left.
        xMax = Math.Max(xMin, xMax);

        _viewMins = new(xMin, yMin);
        _viewMaxes = new(xMax, yMax);
    }

    /// <summary>
    /// Updates the camera's position according to the position of its target.
    /// </summary>
    /// <param name="target">The position of its target.</param>
    public void UpdatePosition (vec2 target) {
        // TODO: Make this parameter adjustable.
        // This is half of the total size of the player entity.
        const float __MARIO_OFFSET = 16f;

        float x = Math.Clamp(target.X + __MARIO_OFFSET, _viewMins.X, _viewMaxes.X);
        float y = Math.Clamp(target.Y + __MARIO_OFFSET, _viewMins.Y, _viewMaxes.Y);

        float xFixed = ((int)(x * 10f) / 10f) + 0.01f;
        float yFixed = ((int)(y * 10f) / 10f) + 0.01f;

        View.Center = new(xFixed, yFixed);
    }

    public void DrawDebugShape (RenderWindow window) {
        RectangleShape relativeCenter = new(new vec2(6f, 6f)) {
            FillColor = Color.Green,
            Position = (vec2)_halfWindowSize
        };

        window.Draw(relativeCenter);
    }
}
