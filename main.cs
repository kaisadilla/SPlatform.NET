global using static splatform.Constants;
global using ivec2 = SFML.System.Vector2i;
global using vec2 = SFML.System.Vector2f;

using splatform;
using splatform.assets;
using splatform.game;
using splatform.window;

const int DEFAULT_ZOOM = 2;

float _cumulativeFixedTime = 0.0f;

// Must be loaded before doing anything else.
Assets.LoadData();
Time.Start();

WindowManager windowMgr = new(DEFAULT_ZOOM);

Game game = new();
game.Init(windowMgr);

while (windowMgr.IsOpen) {
    Time.Update();
    windowMgr.NextFrame();

    _cumulativeFixedTime += Time.DeltaTime;

    #if NO_MULTI_FIXED_UPDATE // Allows the program to be stopped without physics being recalculated later.
    if (_cumulativeFixedTime > SECONDS_PER_FIXED_UPDATE) {
        game.FixedUpdate();
        _cumulativeFixedTime = 0f;
    }
    #else
    while (_cumulativeFixedTime > SECONDS_PER_FIXED_UPDATE) {
        FixedUpdate();
        _cumulativeFixedTime -= SECONDS_PER_FIXED_UPDATE;
    }
    #endif

    game.Update();
    windowMgr.Draw(game);
    game.LateUpdate();
}

game.End(windowMgr);
windowMgr.Close();
