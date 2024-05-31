using Betwixt;

namespace splatform.animation;
internal class TweenAnimation<T> {
    private Tweener<T> _tween;
    private Action? _endCallback;

    private bool _animating = false;
    private bool _loop = false;

    public TweenAnimation (
        Tweener<T> tween, bool loop, Action? endCallback = null
    ) {
        _tween = tween;
        _loop = loop;
        _endCallback = endCallback;

        _tween.OnEnd += (sender, evt) => {
            if (_loop) {
                tween.Reset();
            }
            else {
                _animating = false;
            }

            endCallback?.Invoke();
        };
    }

    public void Start () {
        _animating = true;
    }

    public void Update (Action<T> callback) {
        if (_animating == false) return;

        _tween.Update(Time.DeltaTime);
        T newValue = _tween.Value;
        callback(newValue);
    }
}
