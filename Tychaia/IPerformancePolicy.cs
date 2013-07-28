//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Tychaia
{
    public interface IPerformancePolicy
    {
        /// <summary>
        /// The maximum time in milliseconds that a chunk renderer should
        /// consume before passing control back to the main game operation.
        /// </summary>
        /// <value>The maximum time for chunk rendering to consume, in milliseconds.</value>
        int MaximumChunkRenderingMilliseconds { get; }
    }
}

