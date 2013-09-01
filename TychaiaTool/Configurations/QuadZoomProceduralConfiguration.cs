// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.ProceduralGeneration;

namespace TychaiaTool
{
    public class QuadZoomProceduralConfiguration : IProceduralConfiguration
    {
        private readonly IRuntimeLayerFactory m_RuntimeLayerFactory;

        public QuadZoomProceduralConfiguration(
            IRuntimeLayerFactory runtimeLayerFactory)
        {
            this.m_RuntimeLayerFactory = runtimeLayerFactory;
        }

        public IGenerator GetConfiguration()
        {
            var algorithmZoom1 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmZoom2D());
            var algorithmZoom2 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmZoom2D());
            var algorithmZoom3 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmZoom2D());
            var algorithmZoom4 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmZoom2D());
            var algorithmInitialLand = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmInitialBool());
            algorithmZoom4.SetInput(0, algorithmInitialLand);
            algorithmZoom3.SetInput(0, algorithmZoom4);
            algorithmZoom2.SetInput(0, algorithmZoom3);
            algorithmZoom1.SetInput(0, algorithmZoom2);
            return algorithmZoom1;
        }
    }
}

