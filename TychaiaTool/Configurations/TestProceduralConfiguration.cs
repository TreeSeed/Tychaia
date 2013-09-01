// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.ProceduralGeneration;

namespace TychaiaTool
{
    public class TestProceduralConfiguration : IProceduralConfiguration
    {
        private readonly IRuntimeLayerFactory m_RuntimeLayerFactory;

        public TestProceduralConfiguration(
            IRuntimeLayerFactory runtimeLayerFactory)
        {
            this.m_RuntimeLayerFactory = runtimeLayerFactory;
        }

        public IGenerator GetConfiguration()
        {
            var algorithmTest1 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmPassthrough());
            var algorithmTest2 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmPassthrough());
            var algorithmConstant = this.m_RuntimeLayerFactory.CreateRuntimeLayer(
                new AlgorithmConstant { Constant = 5 });
            algorithmTest2.SetInput(0, algorithmConstant);
            algorithmTest1.SetInput(0, algorithmTest2);
            return algorithmTest1;
        }
    }
}

