using splatform.tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.game.scenes;
internal partial class TileLayer {
    public static TileLayer ReadNext (BinaryReader reader, LevelScene level) {
        bool checkCollisions = reader.ReadBoolean();
        int tileCount = reader.ReadInt32();

        List<OldTile> tiles = new(tileCount);
        for (int i = 0; i < tileCount; i++) {
            OldTile tile = OldTile.ReadNext(reader);
            tile.SetLevel(level);
            tiles.Add(tile);
        }

        return new TileLayer(level) {
            ChecksCollisions = checkCollisions,
            Tiles = tiles,
        };
    }
}
