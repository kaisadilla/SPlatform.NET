using splatform.tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.game.scenes;
internal partial class TileLayer {
    public LevelScene Level { get; private set; }
    public bool ChecksCollisions { get; private set; }
    public List<OldTile> Tiles { get; private set; } = new();

    private TileLayer (LevelScene level) {
        Level = level;
    }
}
