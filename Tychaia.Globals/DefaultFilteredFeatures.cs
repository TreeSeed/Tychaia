//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System.Linq;

namespace Tychaia.Globals
{
    internal class DefaultFilteredFeatures : IFilteredFeatures
    {
        private Feature[] m_PermittedFeatures = null;

        public DefaultFilteredFeatures()
        {
            // Change this to effect what features are enabled in the program.
            this.m_PermittedFeatures = new Feature[]
            {
                Feature.RenderEntities,
                Feature.RenderWorld,
                //Feature.AutomaticChunkValidation,
                Feature.DepthBuffer,
                //Feature.OptimizeChunkProviding,
                //Feature.OptimizeChunkRendering, BUGGY!
                Feature.DiscardChunkTextures,
                //Feature.IsometricOcclusion,
                Feature.RenderingBuffers,
                //Feature.DebugChunkBackground,
                //Feature.DebugChunkTiles,
                //Feature.DebugMovement,
                //Feature.DebugOctreeValidation,
                //Feature.DebugOctreeLookup,
                Feature.RenderCellSides,
                Feature.RenderCellTops
            };
        }

        public bool IsEnabled(Feature feature)
        {
            return this.m_PermittedFeatures.Contains(feature);
        }
    }
}
