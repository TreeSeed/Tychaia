// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.ProceduralGeneration;

namespace TychaiaTool
{
    public class HeightChange3DProceduralConfiguration : IProceduralConfiguration
    {
        private readonly IRuntimeLayerFactory m_RuntimeLayerFactory;

        public HeightChange3DProceduralConfiguration(
            IRuntimeLayerFactory runtimeLayerFactory)
        {
            this.m_RuntimeLayerFactory = runtimeLayerFactory;
        }

        public IGenerator GetConfiguration()
        {
            var constant = this.m_RuntimeLayerFactory.CreateRuntimeLayer(
                new AlgorithmConstant { Constant = 123456 });
            var perlin = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmPerlin());
            var add = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmAdd());
            var perlin2 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmPerlin());
            var passthrough = this.m_RuntimeLayerFactory.CreateRuntimeLayer(
                new AlgorithmPassthrough { XBorder = 7, YBorder = 9, ZBorder = 11 });
            var heightC = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmHeightChange());
            var zoom3D = this.m_RuntimeLayerFactory.CreateRuntimeLayer(new AlgorithmZoom3D());
            passthrough.SetInput(0, perlin2);
            zoom3D.SetInput(0, perlin);
            add.SetInput(0, zoom3D);
            add.SetInput(1, passthrough);
            heightC.SetInput(0, add);
            return heightC;
        }
    }
}

