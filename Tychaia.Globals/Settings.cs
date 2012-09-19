using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Globals
{
    public static class Settings
    {
        public static int ChunkDepth = 128;
    }

    public static class Scale
    {
        public const int CUBE_X = 16;
        public const int CUBE_Y = 16;
        public const int CUBE_Z = 16;
    }

    public static class Performance
    {
        public const int RENDERING_MILLISECONDS = 100;
    }
}
