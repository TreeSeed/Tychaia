// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.ProceduralGeneration;

namespace TychaiaTool
{
    public class TraceTestProceduralConfiguration : IProceduralConfiguration
    {
        private readonly IRuntimeLayerFactory m_RuntimeLayerFactory;

        public TraceTestProceduralConfiguration(
            IRuntimeLayerFactory runtimeLayerFactory)
        {
            this.m_RuntimeLayerFactory = runtimeLayerFactory;
        }

        public IGenerator GetConfiguration()
        {
            var algorithmInitial = new AlgorithmInitialBool();
            var algorithmZoom2DIteration1 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration2 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration3 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration4 = new AlgorithmZoom2D();
            var algorithmIncrementWaterDistance1 = new AlgorithmIncrementWaterDistance()
            {
                Initial = true
            };
            var algorithmIncrementWaterDistance2 = new AlgorithmIncrementWaterDistance();
            var algorithmIncrementWaterDistance3 = new AlgorithmIncrementWaterDistance();
            var algorithmIncrementWaterDistance4 = new AlgorithmIncrementWaterDistance();
            var runtimeInitial = this.m_RuntimeLayerFactory.CreateRuntimeLayer(algorithmInitial);
            var runtimeZoom2DIteration1 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(algorithmZoom2DIteration1);
            var runtimeZoom2DIteration2 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(algorithmZoom2DIteration2);
            var runtimeZoom2DIteration3 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(algorithmZoom2DIteration3);
            var runtimeZoom2DIteration4 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(algorithmZoom2DIteration4);
            var runtimeIncrementWaterDistance1 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(
                algorithmIncrementWaterDistance1);
            var runtimeIncrementWaterDistance2 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(
                algorithmIncrementWaterDistance2);
            var runtimeIncrementWaterDistance3 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(
                algorithmIncrementWaterDistance3);
            var runtimeIncrementWaterDistance4 = this.m_RuntimeLayerFactory.CreateRuntimeLayer(
                algorithmIncrementWaterDistance4);
            runtimeZoom2DIteration1.SetInput(0, runtimeInitial);
            runtimeIncrementWaterDistance1.SetInput(0, runtimeZoom2DIteration1);
            runtimeZoom2DIteration2.SetInput(0, runtimeIncrementWaterDistance1);
            runtimeIncrementWaterDistance2.SetInput(0, runtimeZoom2DIteration2);
            runtimeZoom2DIteration3.SetInput(0, runtimeIncrementWaterDistance2);
            runtimeIncrementWaterDistance3.SetInput(0, runtimeZoom2DIteration3);
            runtimeZoom2DIteration4.SetInput(0, runtimeIncrementWaterDistance3);
            runtimeIncrementWaterDistance4.SetInput(0, runtimeZoom2DIteration4);

            runtimeInitial.Userdata = "runtimeInitial";
            runtimeZoom2DIteration1.Userdata = "runtimeZoom2DIteration1";
            runtimeIncrementWaterDistance1.Userdata = "runtimeIncrementWaterDistance1";
            runtimeZoom2DIteration2.Userdata = "runtimeZoom2DIteration2";
            runtimeIncrementWaterDistance2.Userdata = "runtimeIncrementWaterDistance2";
            runtimeZoom2DIteration3.Userdata = "runtimeZoom2DIteration3";
            runtimeIncrementWaterDistance3.Userdata = "runtimeIncrementWaterDistance3";
            runtimeZoom2DIteration4.Userdata = "runtimeZoom2DIteration4";
            runtimeIncrementWaterDistance4.Userdata = "runtimeIncrementWaterDistance4";

            return runtimeIncrementWaterDistance4;
        }
    }
}
