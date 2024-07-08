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
    public const string PATH_BITFONTS = "res/bitfonts";
    public const string PATH_BITFONT_SPRITES = "res/sprites/bitfonts";
    public const string PATH_ENTITY_SPRITES = "res/sprites/entities";
    public const string PATH_TILE_SPRITES = "res/sprites/tiles";
    public const string PATH_PARTICLE_SPRITES = "res/sprites/particles";
    public const string PATH_UI_SPRITES = "res/sprites/ui";
    public const string PATH_BACKGROUND_IMAGES = "res/sprites/backgrounds";
    public const string PATH_MUSIC = "res/music";
    public const string PATH_SOUNDS = "res/sound";
    #endregion

    #region Original B3 values
    // In B3, speed is measured in units, with each unit being 1 / 4096th of
    // a pixel. Time is measured in frames, with 1 second always having 60 frames.
    /// <summary>
    /// The amount of units in one pixel in B3.
    /// </summary>
    public const float B3_UNITS_PER_PIXEL = 4096f;
    /// <summary>
    /// The amount of frames per second in B3.
    /// </summary>
    public const float B3_FRAMES_PER_SECOND = 60f;
    /// <summary>
    /// The amount of B3 space units in one pixel (SPlatform's space unit).
    /// </summary>
    public const float B3_SPP = 1 / 4096f;
    /// <summary>
    /// The amount of B3 time units in one second (SPlatform's time unit).
    /// </summary>
    public const float B3_TPS = B3_FRAMES_PER_SECOND;
    /// <summary>
    /// A constant that turns B3's units/frame into SP's pixels/second.
    /// </summary>
    public const float B3CONV = B3_SPP * B3_TPS;
    public const float M_MINIMUM_WALK_SPEED = 0x00130 * B3CONV;
    public const float M_MAXIMUM_WALK_SPEED = 0x01900 * B3CONV;
    public const float M_MAXIMUM_WALK_SPEED_UNDERWATER = 0x01100 * B3CONV;
    public const float M_MAXIMUM_WALK_SPEED_LEVEL_ENTRY = 0x00d00 * B3CONV;
    public const float M_MAXIMUM_RUN_SPEED = 0x02900 * B3CONV;
    public const float M_WALK_ACCELERATION = 0x00098 * B3CONV * B3_TPS;
    public const float M_RUN_ACCELERATION = 0x000e4 * B3CONV * B3_TPS;
    public const float M_RELEASE_DECELERATION = 0x000D0 * B3CONV * B3_TPS;
    public const float M_SKID_DECELERATION = 0x001a0 * B3CONV * B3_TPS;
    #endregion
    
    public enum EndianSignature {
        LITTLE_ENDIAN = 0,
        BIG_ENDIAN = 1,
    }
}
