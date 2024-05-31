namespace splatform.animation;
internal class AnimationState {
    private List<Animation> _animations;

    public int State { get; private set; } = 0;

    public Animation CurrentAnimation => _animations[State];

    public AnimationState () {
        _animations = new();
    }

    public AnimationState (List<Animation> animations) {
        _animations = animations;
    }

    public void SetState (int state) {
        if (state != State) {
            State = state;
            _animations[State].Reset();
        }
    }

    public void SetAnimations (List<Animation> animations) {
        _animations = animations;
    }

    public void ClearAnimations () {
        _animations.Clear();
    }

    public void AddAnimation (Animation animation) {
        _animations.Add(animation);
    }

    public void OnUpdate (float deltaTime, float speed) {
        _animations[State].Update(deltaTime, speed);
    }
}
