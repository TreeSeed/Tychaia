// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.ProceduralGeneration;

namespace TychaiaTool
{
    public class DoubleZoomProceduralConfiguration : IProceduralConfiguration
    {
        private readonly IRuntimeLayerFactory m_RuntimeLayerFactory;

        public DoubleZoomProceduralConfiguration(
            IRuntimeLayerFactory runtimeLayerFactory)
        {
            this.m_RuntimeLayerFactory = runtimeLayerFactory;
        }

        public IGenerator GetConfiguration()
        {
            var algorithmZoom1 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmZoom2D());
            var algorithmZoom2 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmZoom2D());
            var algorithmInitialLand = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmInitialBool());
            algorithmZoom2.SetInput(0, algorithmInitialLand);
            algorithmZoom1.SetInput(0, algorithmZoom2);
            return algorithmZoom1;
        }
    }
}

