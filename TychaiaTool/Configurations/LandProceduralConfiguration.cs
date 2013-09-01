// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.ProceduralGeneration;

namespace TychaiaTool
{
    public class LandProceduralConfiguration : IProceduralConfiguration
    {
        private readonly IRuntimeLayerFactory m_RuntimeLayerFactory;

        public LandProceduralConfiguration(
            IRuntimeLayerFactory runtimeLayerFactory)
        {
            this.m_RuntimeLayerFactory = runtimeLayerFactory;
        }

        public IGenerator GetConfiguration()
        {
            return this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmInitialBool());
        }
    }
}

