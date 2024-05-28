﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform;
public static class Constants {
    public const int WINDOW_WIDTH_IN_TILES = 24;
    public const int WINDOW_HEIGHT_IN_TILES = 14;

    public const int PIXELS_PER_TILE = 16;

    public const float SECONDS_PER_FIXED_UPDATE = 0.01f;

    public const float DEAD_ENEMY_DESPAWN_TIME = 0.25f;

    #region Paths
    public const string PATH_REGISTRY = "res/data/registry.json";
    public const string PATH_TILE_SPRITES = "res/sprites/tiles";
    public const string PATH_PARTICLE_SPRITES = "res/sprites/particles";
    public const string PATH_BACKGROUND_IMAGES = "res/sprites/backgrounds";
    public const string PATH_MUSIC = "res/music";
    public const string PATH_SOUNDS = "res/sound";
    #endregion
}
