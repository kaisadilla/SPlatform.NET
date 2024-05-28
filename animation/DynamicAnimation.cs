using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.animation;
internal class DynamicAnimation : Animation {
    private int _frameCount;
    private int _currentFrame = 0;
    private float _timeSinceLastFrame = 0f;

    private List<float> _frameTimes;
    private List<IntRect> _frameSlices = new();
    private List<IntRect> _frameSlicesMirrored = new();

    private bool _hasCallback = false;

    public bool Loop { get; set; } = true;
    public Action? Callback { get; set; } = null;

    public DynamicAnimation (
        ivec2 slices, vec2 sliceSize, List<float> frameTimes, List<int> frames
    ) {
        if (frameTimes.Count != frames.Count) throw new ArgumentException(
            "The amount of frames given and the amount of frametimes " + 
            "given are not the same."
        );

        _frameTimes = frameTimes;

        Init(frames, slices, sliceSize);
    }

    public DynamicAnimation (
        ivec2 slices, vec2 sliceSize, float frameTime, List<int> frames
    ) {
        _frameTimes = new();

        for (int i = 0; i < frames.Count; i++) {
            _frameTimes.Add(frameTime);
        }

        Init(frames, slices, sliceSize);
    }

    public DynamicAnimation (
        ivec2 slices, vec2 sliceSize, float frameTime, List<int> frames,
        Action callback
    ) {
        _frameTimes = new();
        _hasCallback = true;
        Callback = callback;

        for (int i = 0; i < frames.Count; i++) {
            _frameTimes.Add(frameTime);
        }

        Init(frames, slices, sliceSize);
    }

    public override void OnUpdate (float deltaTime, float speed) {
        _timeSinceLastFrame += deltaTime * speed;

        while (_timeSinceLastFrame >= _frameTimes[_currentFrame]) {
            _timeSinceLastFrame -= _frameTimes[_currentFrame];

            _currentFrame++;

            if (_currentFrame >= _frameCount) {
                Callback?.Invoke();

                if (Loop) {
                    _currentFrame = 0;
                }
            }
        }
    }

    public override void Reset () {
        _currentFrame = 0;
        _timeSinceLastFrame = 0f;
    }

    public override IntRect GetCurrentSlice (bool mirrored) {
        return mirrored
            ? _frameSlicesMirrored[_currentFrame]
            : _frameSlices[_currentFrame];
    }

    private void Init (List<int> frames, ivec2 slices, vec2 sliceSize) {
        int totalFrames = slices.X * slices.Y;

        foreach (int frame in frames) {
            if (frame >= totalFrames) throw new ArgumentException(
                $"Frame '{frame}' is outside the bounds of the texture"
            );

            IntRect slice = CalculateSliceDimensions(slices, sliceSize, frame);

            _frameSlices.Add(slice);
            _frameSlicesMirrored.Add(GetMirroredSlice(slice));
        }

        _frameCount = _frameSlices.Count;
    }
}
