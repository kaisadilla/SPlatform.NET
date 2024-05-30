using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform;
internal static class Debug {
    public static bool DisplayStats { get; set; } = false;
    public static bool DrawColliders { get; set; } = false; // old name: drawCollisions
    public static bool DrawVisualInfo { get; set; } = false; // old name: drawDebugInfo
}
