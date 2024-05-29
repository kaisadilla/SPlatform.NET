using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.tiles;
internal class QuestionBlock : Tile {
    public enum ContentType {
        Coin = 0,
        Entity = 1,
        Tile = 2,
    }

    public enum HitMode {
        Once = 0,
        Times = 1, // n times
        Seconds = 2, // n seconds
    }

    public enum State {
        Active = 0,
        Empty = 1,
    }

    // TOO: QuestionBlock
}
