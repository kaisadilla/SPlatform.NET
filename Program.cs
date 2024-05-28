global using static splatform.Constants;
global using vec2 = SFML.System.Vector2f;
global using ivec2 = SFML.System.Vector2i;
global using uvec2 = SFML.System.Vector2u;

using SFML.Graphics;
using splatform.game;
using splatform.assets;
using splatform.game.scenes;
using System.IO;

// Must be loaded before doing anything else.
Assets.LoadData();

Game game = new();
game.Init();

while (game.IsOpen) {
    game.Update();
    game.Draw();
    game.LateUpdate();
}

game.Close();