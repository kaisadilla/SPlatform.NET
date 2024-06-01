using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform;

public static class Extensions {
    public static Direction Opposite (this Direction direction) {
        if (direction == Direction.Up) return Direction.Down;
        if (direction == Direction.Down) return Direction.Up;
        if (direction == Direction.Left) return Direction.Right;
        if (direction == Direction.Right) return Direction.Left;

        return direction;
    }
    public static vec2 ToVec2 (this float[] arr) {
        return new(arr[0], arr[1]);
    }

    public static ivec2 ToIvec2 (this int[] arr) {
        return new(arr[0], arr[1]);
    }

    public static uvec2 ToUvec2 (this uint[] arr) {
        return new(arr[0], arr[1]);
    }

    public static FloatRect ToFloatRect (this float[] arr) {
        return new(arr[0], arr[1], arr[2], arr[3]);
    }

    public static IntRect ToIntRect (this int[] arr) {
        return new(arr[0], arr[1], arr[2], arr[3]);
    }
}
