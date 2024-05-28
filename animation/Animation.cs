using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.animation;
internal abstract class Animation {
    public abstract void OnUpdate(float deltaTime, float speed);
    public abstract void Reset();

    public abstract IntRect GetCurrentSlice(bool mirrored);

    /// <summary>
    /// Given information about the spritesheet and the position of the slice
    /// inside it, returns the coordinates of that slice.
    /// </summary>
    /// <param name="slices">The amount of sprites in the x and y coordinates
    /// within the spritesheet.</param>
    /// <param name="sliceSize">The size of each sprite, in pixels.</param>
    /// <param name="index">The index of the slice, left to right, then top to
    /// bottom.</param>
    /// <returns></returns>
    protected IntRect CalculateSliceDimensions (
        ivec2 slices, vec2 sliceSize, int index
    ) {
        int xStart = index % slices.X;
        int yStart = index / slices.X;

        return new(
            (int)(xStart * sliceSize.X),
            (int)(yStart * sliceSize.Y),
            (int)sliceSize.X,
            (int)sliceSize.Y
        );
    }

    /// <summary>
    /// Returns the coordinates of the slice given, but mirrored horizontally.
    /// </summary>
    /// <param name="slice">The coordinates of the slice.</param>
    /// <returns></returns>
    protected IntRect GetMirroredSlice (IntRect slice) {
        return new(
            slice.Left + slice.Width,
            slice.Top,
            -slice.Width,
            slice.Height
        );
    }
}
