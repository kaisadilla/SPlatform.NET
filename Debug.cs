using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform;
internal static class Debug {
    public static bool ShowDebugInfo { get; set; } = false;
    public static bool DisplayColliders { get; set; } = false; // old name: drawCollisions
    public static bool ShowDebugShapes { get; set; } = false; // old name: drawDebugInfo
}
