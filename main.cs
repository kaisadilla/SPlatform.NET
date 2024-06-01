global using static splatform.Constants;
global using uvec2 = SFML.System.Vector2u;
global using ivec2 = SFML.System.Vector2i;
global using vec2 = SFML.System.Vector2f;

using splatform;
using splatform.assets;
using splatform.game;
using splatform.window;
using splatform.window.ui;
using splatform.game.scenes;

const int DEFAULT_ZOOM = 2;

float _cumulativeFixedTime = 0.0f;

// Must be loaded before doing anything else.
Assets.LoadData();
Time.Start();

BitmapFont font = new(PATH_BITFONTS + "/smb3-font.json");

WindowManager windowMgr = new(DEFAULT_ZOOM);

Game game = new();
game.Init(windowMgr);

LevelUi levelUi = new(DEFAULT_ZOOM);

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
    levelUi.UpdateInfo(game.Context, (LevelScene)game.Scene);
    windowMgr.Draw(game, levelUi);
    game.LateUpdate();
}

game.End(windowMgr);
windowMgr.Close();
