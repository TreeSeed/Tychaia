// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using Tychaia.Globals;

namespace TychaiaWorldGenViewerAlgorithm
{
    public class RenderingLocationProvider : IRenderingLocationProvider
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }
    }
}