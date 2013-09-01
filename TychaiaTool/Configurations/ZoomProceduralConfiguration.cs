// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.ProceduralGeneration;

namespace TychaiaTool
{
    public class ZoomProceduralConfiguration : IProceduralConfiguration
    {
        private readonly IRuntimeLayerFactory m_RuntimeLayerFactory;

        public ZoomProceduralConfiguration(
            IRuntimeLayerFactory runtimeLayerFactory)
        {
            this.m_RuntimeLayerFactory = runtimeLayerFactory;
        }

        public IGenerator GetConfiguration()
        {
            var runtime = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmZoom2D());
            runtime.SetInput(0, this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmInitialBool()));
            return runtime;
        }
    }
}

