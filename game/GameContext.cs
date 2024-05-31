using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.game;
internal class GameContext {
    public int Lives { get; set; } = 5;
    public int Coins { get; set; } = 0;
    public int Score { get; set; } = 0;
    public int YoshiCoins { get; set; } = 0;
    public int[] EndLevelItems { get; set; } = [0, 0, 0];
}
