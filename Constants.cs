using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform;
public static class Constants {
    public const string WINDOW_TITLE = "SPlatform";

    public const int WINDOW_WIDTH_IN_TILES = 24;
    public const int WINDOW_HEIGHT_IN_TILES = 14;

    public const int PIXELS_PER_TILE = 16;

    public const float SECONDS_PER_FIXED_UPDATE = 0.01f;

    public const float DEAD_ENEMY_DESPAWN_TIME = 0.25f;

    /// <summary>
    /// The maximum speed (in any axis) that an entity can carry while still
    /// considered "still". This accounts for small errors where an entity
    /// that is supposed to be still has a velocity slightly different to 0.
    /// </summary>
    public const float STILL_ENTITY_THRESHOLD = 0.1f;

    #region Paths
    public const string PATH_LEVELS = "res/data/levels";
    public const string PATH_REGISTRY = "res/data/registry.json";
    public const string PATH_ENTITY_SPRITES = "res/sprites/entities";
    public const string PATH_TILE_SPRITES = "res/sprites/tiles";
    public const string PATH_PARTICLE_SPRITES = "res/sprites/particles";
    public const string PATH_BACKGROUND_IMAGES = "res/sprites/backgrounds";
    public const string PATH_MUSIC = "res/music";
    public const string PATH_SOUNDS = "res/sound";
    #endregion
}
