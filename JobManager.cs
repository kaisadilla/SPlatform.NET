namespace splatform;
internal static class JobManager {
    private static List<Job> _pendingJobs = new();

    public static void OnUpdate () {
        for (int i = _pendingJobs.Count - 1; i >= 0; i--) {
            Job job = _pendingJobs[i];
            job.ReduceTimer(Time.DeltaTime);

            if (job.Timer <= 0) {
                job.Callback();
                _pendingJobs.RemoveAt(i);
            }
        }
    }

    public static void AddJob (Job job) {
        _pendingJobs.Add(job);
    }

    public static void AddJob (float delay, Action callback) {
        _pendingJobs.Add(new(delay, callback));
    }
}
