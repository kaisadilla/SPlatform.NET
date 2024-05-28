using Newtonsoft.Json;
using splatform.assets;
using splatform.assets.data;
using splatform.tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.game.scenes;
internal partial class LevelScene {
    public static LevelScene FromJson (string json) {
        LevelData? data = JsonConvert.DeserializeObject<LevelData>(json);

        if (data == null) {
            throw new Exception("Couldn't read json file.");
        }

        LevelScene scene = new() {
            Width = data.Width,
            Height = data.Height,
            _backgroundLayer = ReadJsonLayer(
                data.Grids.Background, data.Width, data.Height, false
            ),
            _foregroundLayer = ReadJsonLayer(
                data.Grids.Foreground, data.Width, data.Height, false
            ),
        };

        scene.LoadBackground(data.Background);
        scene.LoadMusic(data.Music);

        return scene;
    }

    private static List<Tile> ReadJsonLayer (
        Dictionary<string, string> layer,
        int width,
        int height,
        bool generateColliders
    ) {
        List<Tile> tiles = new();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                string sprite = layer[$"{x},{y}"];

                if (sprite == "air") continue;

                int spriteIndex = Assets.Registry!.TileSpriteIndices[sprite];
                int tileIndex = Assets.TileIndices[spriteIndex][0];

                int xStart = tileIndex % Assets.TexturesPerRow;
                int yStart = tileIndex / Assets.TexturesPerRow;

                Tile tile = new() {
                    __debug_Slice = new(
                        xStart * Assets.TileSize,
                        yStart * Assets.TileSize,
                        Assets.TileSize,
                        Assets.TileSize
                    )
                };
                tile.SetGridPosition(new(x, y));

                tiles.Add(tile);
            }
        }

        return tiles;
    }
}
