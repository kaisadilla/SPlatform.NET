namespace splatform;
internal class Job {
    public float Timer { get; private set; }
    public Action Callback { get; private set; }

    public Job (float delay, Action callback) {
        Timer = delay;
        Callback = callback;
    }

    public void ReduceTimer (float amount) {
        Timer -= amount;
    }
}
