//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Protogame;

namespace Tychaia
{
    /// <summary>
    /// This intelligence component updates the rendering pipeline to cancel
    /// requests to chunks that no longer require rendering, as well as chunks
    /// that can be discarded or paged to disk.
    /// </summary>
    public class RenderingOptimizerComponent : IIntelligenceComponent
    {
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }
    }
}

