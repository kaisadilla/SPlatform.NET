global using static splatform.Constants;
global using ivec2 = SFML.System.Vector2i;
global using vec2 = SFML.System.Vector2f;

using splatform;
using splatform.assets;
using splatform.game;

// Must be loaded before doing anything else.
Assets.LoadData();
Time.Start();

Game game = new();
game.Init();

while (game.IsOpen) {
    Time.Update();

    game.Update();
    game.Draw();
    game.LateUpdate();
}

game.Close();
