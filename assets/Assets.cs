#define ASSERT_REGISTRY

using SFML.Audio;
using SFML.Graphics;

namespace splatform.assets;
internal static class Assets {
    public static Registry? Registry { get; private set; }

    public static Texture TileAtlas { get; private set; } = new(1, 1);
    /// <summary>
    /// Stores the position of each tile sprite frame inside the array. The key
    /// is a ivec2 containing the index of the tile sprite and the index of
    /// the frame. For static sprites, the index of the frame will always be 0.
    /// </summary>
    public static List<List<int>> TileIndices { get; private set; } = new();
    public static int TileSize { get; private set; }
    public static int TexturesPerRow { get; private set; }
    public static float NormalizedTextureSize { get; private set; } // TODO: Remove / ?

    public static Dictionary<string, Texture> ParticleTextures = new();

    public static _SoundAssets? Sounds { get; private set; }
    
    public static void LoadData () {
        Registry = new Registry(PATH_REGISTRY);

        BuildTileAtlas();
        LoadSounds();
        LoadParticleTextures();
    }

    public static string GetBackgroundImageAt (int index) {
        #if ASSERT_REGISTRY
        if (Registry == null) throw _NoRegistryException();
        #endif

        return Registry!.BackgroundImages[index];
    }

    public static string GetMusicAt (int index) {
        #if ASSERT_REGISTRY
        if (Registry == null) throw _NoRegistryException();
        #endif

        return Registry!.Music[index];
    }

    public static string GetEntitySpriteAt (int index) {
        #if ASSERT_REGISTRY
        if (Registry == null) throw _NoRegistryException();
        #endif

        return Registry!.EntitySprites[index];
    }

    public static string GetParticleSpriteAt (int index) {
        #if ASSERT_REGISTRY
        if (Registry == null) throw _NoRegistryException();
        #endif

        return Registry!.ParticleSprites[index];
    }

    private static void BuildTileAtlas () {
        if (Registry == null) throw _NoRegistryException();

        TileSize = Registry.TileSize;

        Console.WriteLine("=== BUILDING TILE ATLAS ===");

        List<Image> tiles = new();
        int tileCount = 0;

        foreach (string imgName in Registry.TileSprites) {
            Image image = new(PATH_TILE_SPRITES + "/" + imgName + ".png");

            // TODO: Tiles cannot be bigger than int.MaxValue.
            int sheetWidth = (int)image.Size.X / Registry.TileSize;
            int sheetHeight = (int)image.Size.Y / Registry.TileSize;

            tileCount += sheetWidth * sheetHeight;
            tiles.Add(image);
        }

        int atlasRows = 24;
        uint atlasWidth = (uint)atlasRows * (uint)TileSize;

        Image atlas = new(atlasWidth, atlasWidth, new Color(0, 0, 0, 0));

        int atlasIndex = 0;

        for (int i = 0; i < tiles.Count; i++) {
            Image tile = tiles[i];

            int frame = 0;
            TileIndices.Add(new());

            // for each frame in the spritesheet.
            for (uint xFrame = 0; xFrame < tile.Size.X; xFrame += (uint)TileSize) {
                for (uint yFrame = 0; yFrame < tile.Size.Y; yFrame += (uint)TileSize) {
                    uint rowPos = (uint)atlasIndex / (uint)atlasRows;
                    uint colPos = (uint)atlasIndex - (rowPos * (uint)atlasRows);

                    uint xStartInAtlas = colPos * (uint)TileSize;
                    uint yStartInAtlas = rowPos * (uint)TileSize;

                    uint xStartInTile = xFrame;
                    uint yStartInTile = yFrame;

                    // for each pixel in the current frame.
                    // TODO: Not working as expected
                    for (uint y = 0; y < TileSize; y++) {
                        uint yAtlas = yStartInAtlas + y;

                        for (uint x = 0; x < TileSize; x++) {
                            uint xAtlas = xStartInAtlas + x;
                            atlas.SetPixel(
                                xAtlas,
                                yAtlas,
                                tile.GetPixel(xStartInTile + x, yStartInTile + y)
                            );
                        }
                    }

                    TileIndices[i].Add(atlasIndex);
                    atlasIndex++;
                    frame++;
                }
            }
        }

        atlas.SaveToFile("res/sprites/tile_atlas_v2.png");

        TexturesPerRow = atlasRows;
        NormalizedTextureSize = 1f / TexturesPerRow;

        TileAtlas = new(atlas);

        Console.WriteLine("Tile atlas created!");
    }

    private static void LoadSounds () {
        Console.WriteLine("=== LOADING SOUNDS ===");

        Sounds = new() {
            Pause = new(PATH_SOUNDS + "/pause.wav"),
            Jump = new(PATH_SOUNDS + "/jump.wav"),
            Stomp = new(PATH_SOUNDS + "/stomp.wav"),
            Kick = new(PATH_SOUNDS + "/kick.wav"),
            PlayerDeath = new(PATH_SOUNDS + "/player_death.wav"),
            Coin = new(PATH_SOUNDS + "/coin.wav"),
            ItemSpawn = new(PATH_SOUNDS + "/item_spawn.wav"),
        };
    }

    private static void LoadParticleTextures () {
        if (Registry == null) throw _NoRegistryException();

        Console.WriteLine("=== LOADING PARTICLES ===");

        foreach (string fileName in Registry.ParticleSprites) {
            ParticleTextures[fileName] = new(
                PATH_PARTICLE_SPRITES + "/" + fileName + ".png"
            );
        }
    }

    private static Exception _NoRegistryException () {
        return new Exception(
            "Cannot access assets before the registry has been built."
        );
    }
}

internal class _SoundAssets {
    public required SoundBuffer Pause { get; init; }
    public required SoundBuffer Jump { get; init; }
    public required SoundBuffer Stomp { get; init; }
    public required SoundBuffer Kick { get; init; }
    public required SoundBuffer PlayerDeath { get; init; }
    public required SoundBuffer Coin { get; init; }
    public required SoundBuffer ItemSpawn { get; init; }
}
