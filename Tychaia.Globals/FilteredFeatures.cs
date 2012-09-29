using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tychaia.Globals
{
    public static class FilteredFeatures
    {
        private static Feature[] m_PermittedFeatures = null;

        static FilteredFeatures()
        {
            // Change this to effect what features are enabled in the program.
            m_PermittedFeatures = new Feature[]
            {
                Feature.RenderEntities,
                Feature.RenderWorld,
                //Feature.AutomaticChunkValidation,
                Feature.DepthBuffer,
                Feature.OptimizeChunkProviding,
                Feature.OptimizeChunkRendering,
                Feature.DiscardChunkTextures,
                Feature.IsometricOcclusion,
                Feature.RenderingBuffers,
                //Feature.DebugChunkBackground,
                //Feature.DebugChunkTiles,
                //Feature.DebugMovement
                Feature.RenderCellSides,
                Feature.RenderCellTops
            };
        }

        public static bool IsEnabled(Feature feature)
        {
            return m_PermittedFeatures.Contains(feature);
        }
    }

    public enum Feature
    {
        RenderEntities,
        RenderWorld,
        AutomaticChunkValidation,
        DepthBuffer,
        OptimizeChunkProviding,
        OptimizeChunkRendering,
        DiscardChunkTextures,
        IsometricOcclusion,
        RenderingBuffers,
        DebugChunkBackground,
        DebugChunkTiles,
        DebugMovement,
        RenderCellSides,
        RenderCellTops,
    }
}
