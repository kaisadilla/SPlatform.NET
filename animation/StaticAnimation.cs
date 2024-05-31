using SFML.Graphics;

namespace splatform.animation;
internal class StaticAnimation : Animation {
    IntRect _frameSlice;
    IntRect _frameSliceMirrored;

    public StaticAnimation (ivec2 slices, vec2 sliceSize, int frame) {
        _frameSlice = CalculateSliceDimensions(slices, sliceSize, frame);
        _frameSliceMirrored = GetMirroredSlice(_frameSlice);
    }

    public override void Update (float deltaTime, float speed) {
        // nothing
    }

    public override void Reset () {
        // nothing
    }

    public override IntRect GetCurrentSlice (bool mirrored) {
        return mirrored ? _frameSliceMirrored : _frameSlice;
    }

}
