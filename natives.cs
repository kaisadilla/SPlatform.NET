using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform;

public enum Direction {
    Up,
    Down,
    Left,
    Right,
    None,
}

public static class Extensions {
    public static Direction Opposite (this Direction direction) {
        if (direction == Direction.Up) return Direction.Down;
        if (direction == Direction.Down) return Direction.Up;
        if (direction == Direction.Left) return Direction.Right;
        if (direction == Direction.Right) return Direction.Left;

        return direction;
    }
}