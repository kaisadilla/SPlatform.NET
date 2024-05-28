using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform;
internal static class Time {
    public static float CurrentTime { get; private set; } = 0.0f;
    public static float DeltaTime { get; private set; } = 0.0f;
    
    public static float TimeScale { get; private set; } = 1.0f;
    public static bool Paused { get; private set; } = true;

    private static Clock _clock = new();

    public static void SetTimeScale (float timeScale) {
        TimeScale = timeScale;
    }

    public static void Pause () {
        Paused = true;
    }

    public static void Resume () {
        Paused = false;
    }

    public static void PauseOrResume () {
        Paused = !Paused;
    }

    public static void Start () {
        _clock.Restart();
        CurrentTime = 0;
        DeltaTime = 0;
        Paused = false;
    }

    public static void Update () {
        DeltaTime = _clock.Restart().AsSeconds() * TimeScale;

        if (Paused) {
            DeltaTime = 0.0f;
        }

        CurrentTime += DeltaTime;
    }
}
