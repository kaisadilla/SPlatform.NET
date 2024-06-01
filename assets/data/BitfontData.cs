using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.assets.data;
internal class BitfontData {
    public string FontName { get; init; }
    public string Spritesheet { get; init; }
    public int[] SpritesheetDimensions { get; init; }
    public int[] CellDimensions { get; init; }
    public int[] CellOffset { get; init; }
    public float[] Spacing { get; init; }
    public int SpaceLength { get; init; }
    public BitfontCharacterData[] Characters { get; init; }
}

internal class BitfontCharacterData {
    public char Character { get; init; }
    public int Index { get; init; }
    public int[]? Offset { get; init; }
}
