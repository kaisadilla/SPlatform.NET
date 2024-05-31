using Betwixt;

namespace splatform.animation;
internal class TweenAnimation<T> {
    /// <summary>
    /// A list of tweens, in order, to play for this animation.
    /// </summary>
    private List<Tweener<T>> _tweens;
    /// <summary>
    /// A function to call when this animation is complete.
    /// </summary>
    private Action? _endCallback;

    /// <summary>
    /// True if this animation is currently being animated (updated).
    /// </summary>
    private bool _animating = false;
    /// <summary>
    /// True if this animation restarts after it ends.
    /// </summary>
    private bool _loop = false;

    /// <summary>
    /// The tween currently active, chosen from the list of tweens.
    /// </summary>
    private Tweener<T> _activeTween;

    public TweenAnimation (
        Tweener<T> tween, bool loop, Action? endCallback = null
    ) {
        _tweens = [tween];
        _activeTween = tween;
        _loop = loop;
        _endCallback = endCallback;

        _activeTween.OnEnd += (sender, evt) => {
            if (_loop) {
                tween.Reset();
                tween.Start();
            }
            else {
                _animating = false;
            }

            endCallback?.Invoke();
        };
    }

    public TweenAnimation (
        List<Tweener<T>> tweens, bool loop, Action? endCallback = null
    ) {
        if (tweens.Count == 0) throw new ArgumentException(
            "TweenAnimation requires at least one Tween."
        );

        _tweens = tweens;
        _activeTween = _tweens[0];
        _loop = loop;
        _endCallback = endCallback;

        for (int i = 0; i < _tweens.Count; i++) {
            // This is the last tween.
            if (i == _tweens.Count - 1) {
                _tweens[i].OnEnd += (sender, evt) => {
                    _activeTween = _tweens[0];
                    _activeTween.Reset();
                    _activeTween.Start();

                    if (_loop == false) {
                        _animating = false;
                    }
                    Console.WriteLine("Last tween's end");
                    _endCallback?.Invoke();
                };
            }
            else {
                int nextIndex = i + 1; // can't use i inside the lambda, since the for loop can change its value.
                _tweens[i].OnEnd += (sender, evt) => {
                    _activeTween = _tweens[nextIndex];
                    _activeTween.Reset();
                    _activeTween.Start();
                };
            }
        }
    }

    #region Setters
    public void SetCallback (Action? endCallback) {
        _endCallback = endCallback;
    }
    #endregion

    #region Interactions
    /// <summary>
    /// Starts this animation.
    /// </summary>
    public void Play () {
        _activeTween.Reset();
        _animating = true;
    }

    /// <summary>
    /// Stops this animation.
    /// </summary>
    public void Stop () {
        _animating = false;
    }

    /// <summary>
    /// Resets this animation (stopping it).
    /// </summary>
    public void Reset () {
        _animating = false;
        _activeTween = _tweens[0];
        _activeTween.Reset();
    }

    /// <summary>
    /// Restarts this animation (equivalent to Reset then Play).
    /// </summary>
    public void Restart () {
        _activeTween = _tweens[0];
        _activeTween.Reset();
        _animating = true;
    }
    #endregion

    /// <summary>
    /// Updates the tween for this frame, and calls a function with the tween's
    /// current value as its argument.
    /// </summary>
    /// <param name="valueCallback">A function to be called after the update</param>
    public void Update (Action<T> valueCallback) {
        if (_animating == false) return;

        _activeTween.Update(Time.DeltaTime); // Note: Tween durations are considered to be in seconds.
        T newValue = _activeTween.Value;
        valueCallback(newValue);
    }
}
