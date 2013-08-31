// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Xunit;

namespace Tychaia.ProceduralGeneration.Tests
{
    public class ContinuityTests : TestBase
    {
        public void PerformSampling(string name, IGenerator layer)
        {
            PerformSamplingIndividual(name, layer, 5, 0);
            PerformSamplingIndividual(name, layer, 5, 100);
            PerformSamplingIndividual(name, layer, 5, 10000);
            PerformSamplingIndividual(name, layer, 5, 10000000);
        }

        private void PerformSamplingIndividual(string name, IGenerator layer, int s, int o)
        {
            int computations;
            int[] initial = layer.GenerateData(-s + o, -s + o, -s + o, s*2, s*2, s*2, out computations);

            for (var x = -s; x < s; x++)
                for (var y = -s; y < s; y++)
                    for (var z = -s; z < s; z++)
                    {
                        int[] sample = layer.GenerateData(x + o, y + o, z + o, 1, 1, 1, out computations);
                        Assert.Equal(sample[0], initial[(x + s) + (y + s)*s*2 + (z + s)*s*2*s*2]); //,
                        //name + " is not continuous.");
                    }
        }

        [Fact]
        public void TestAlgorithmInitialBool()
        {
            var algorithm = new AlgorithmInitialBool();
            var runtime = this.CreateRuntimeLayer(algorithm);
            PerformSampling("AlgorithmInitialBool", runtime);
        }

        [Fact]
        public void TestAlgorithmZoom2DIteration1()
        {
            var algorithmInitial = new AlgorithmInitialBool();
            var algorithmZoom2DIteration1 = new AlgorithmZoom2D();
            var runtimeInitial = this.CreateRuntimeLayer(algorithmInitial);
            var runtimeZoom2DIteration1 = this.CreateRuntimeLayer(algorithmZoom2DIteration1);
            runtimeZoom2DIteration1.SetInput(0, runtimeInitial);
            PerformSampling("AlgorithmZoom2D (1 iteration)", runtimeZoom2DIteration1);
        }

        [Fact]
        public void TestAlgorithmZoom2DIteration2()
        {
            var algorithmInitial = new AlgorithmInitialBool();
            var algorithmZoom2DIteration1 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration2 = new AlgorithmZoom2D();
            var runtimeInitial = this.CreateRuntimeLayer(algorithmInitial);
            var runtimeZoom2DIteration1 = this.CreateRuntimeLayer(algorithmZoom2DIteration1);
            var runtimeZoom2DIteration2 = this.CreateRuntimeLayer(algorithmZoom2DIteration2);
            runtimeZoom2DIteration1.SetInput(0, runtimeInitial);
            runtimeZoom2DIteration2.SetInput(0, runtimeInitial);
            PerformSampling("AlgorithmZoom2D (2 iteration)", runtimeZoom2DIteration2);
        }

        [Fact]
        public void TestAlgorithmIncrementWaterDistance1()
        {
            var algorithmInitial = new AlgorithmInitialBool();
            var algorithmIncrementWaterDistance = new AlgorithmIncrementWaterDistance
            {
                Initial = true
            };
            var runtimeInitial = this.CreateRuntimeLayer(algorithmInitial);
            var runtimeIncrementWaterDistance = this.CreateRuntimeLayer(algorithmIncrementWaterDistance);
            runtimeIncrementWaterDistance.SetInput(0, runtimeInitial);
            PerformSampling("AlgorithmIncrementWaterDistance", runtimeIncrementWaterDistance);
        }

        [Fact]
        public void TestAlgorithmIncrementWaterDistance2()
        {
            var algorithmInitial = new AlgorithmInitialBool();
            var algorithmZoom2DIteration1 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration2 = new AlgorithmZoom2D();
            var algorithmIncrementWaterDistance1 = new AlgorithmIncrementWaterDistance
            {
                Initial = true
            };
            var algorithmIncrementWaterDistance2 = new AlgorithmIncrementWaterDistance();
            var runtimeInitial = this.CreateRuntimeLayer(algorithmInitial);
            var runtimeZoom2DIteration1 = this.CreateRuntimeLayer(algorithmZoom2DIteration1);
            var runtimeZoom2DIteration2 = this.CreateRuntimeLayer(algorithmZoom2DIteration2);
            var runtimeIncrementWaterDistance1 = this.CreateRuntimeLayer(algorithmIncrementWaterDistance1);
            var runtimeIncrementWaterDistance2 = this.CreateRuntimeLayer(algorithmIncrementWaterDistance2);
            runtimeZoom2DIteration1.SetInput(0, runtimeInitial);
            runtimeIncrementWaterDistance1.SetInput(0, runtimeZoom2DIteration1);
            runtimeZoom2DIteration2.SetInput(0, runtimeIncrementWaterDistance1);
            runtimeIncrementWaterDistance2.SetInput(0, runtimeZoom2DIteration2);
            PerformSampling("AlgorithmIncrementWaterDistance (4 iterations)", runtimeIncrementWaterDistance2);
        }

        [Fact]
        public void TestAlgorithmIncrementWaterDistance3()
        {
            var algorithmInitial = new AlgorithmInitialBool();
            var algorithmZoom2DIteration1 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration2 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration3 = new AlgorithmZoom2D();
            var algorithmIncrementWaterDistance1 = new AlgorithmIncrementWaterDistance
            {
                Initial = true
            };
            var algorithmIncrementWaterDistance2 = new AlgorithmIncrementWaterDistance();
            var algorithmIncrementWaterDistance3 = new AlgorithmIncrementWaterDistance();
            var runtimeInitial = this.CreateRuntimeLayer(algorithmInitial);
            var runtimeZoom2DIteration1 = this.CreateRuntimeLayer(algorithmZoom2DIteration1);
            var runtimeZoom2DIteration2 = this.CreateRuntimeLayer(algorithmZoom2DIteration2);
            var runtimeZoom2DIteration3 = this.CreateRuntimeLayer(algorithmZoom2DIteration3);
            var runtimeIncrementWaterDistance1 = this.CreateRuntimeLayer(algorithmIncrementWaterDistance1);
            var runtimeIncrementWaterDistance2 = this.CreateRuntimeLayer(algorithmIncrementWaterDistance2);
            var runtimeIncrementWaterDistance3 = this.CreateRuntimeLayer(algorithmIncrementWaterDistance3);
            runtimeZoom2DIteration1.SetInput(0, runtimeInitial);
            runtimeIncrementWaterDistance1.SetInput(0, runtimeZoom2DIteration1);
            runtimeZoom2DIteration2.SetInput(0, runtimeIncrementWaterDistance1);
            runtimeIncrementWaterDistance2.SetInput(0, runtimeZoom2DIteration2);
            runtimeZoom2DIteration3.SetInput(0, runtimeIncrementWaterDistance2);
            runtimeIncrementWaterDistance3.SetInput(0, runtimeZoom2DIteration3);
            PerformSampling("AlgorithmIncrementWaterDistance (4 iterations)", runtimeIncrementWaterDistance3);
        }

        [Fact]
        public void TestAlgorithmIncrementWaterDistance4()
        {
            var algorithmInitial = new AlgorithmInitialBool();
            var algorithmZoom2DIteration1 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration2 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration3 = new AlgorithmZoom2D();
            var algorithmZoom2DIteration4 = new AlgorithmZoom2D();
            var algorithmIncrementWaterDistance1 = new AlgorithmIncrementWaterDistance
            {
                Initial = true
            };
            var algorithmIncrementWaterDistance2 = new AlgorithmIncrementWaterDistance();
            var algorithmIncrementWaterDistance3 = new AlgorithmIncrementWaterDistance();
            var algorithmIncrementWaterDistance4 = new AlgorithmIncrementWaterDistance();
            var runtimeInitial = this.CreateRuntimeLayer(algorithmInitial);
            var runtimeZoom2DIteration1 = this.CreateRuntimeLayer(algorithmZoom2DIteration1);
            var runtimeZoom2DIteration2 = this.CreateRuntimeLayer(algorithmZoom2DIteration2);
            var runtimeZoom2DIteration3 = this.CreateRuntimeLayer(algorithmZoom2DIteration3);
            var runtimeZoom2DIteration4 = this.CreateRuntimeLayer(algorithmZoom2DIteration4);
            var runtimeIncrementWaterDistance1 = this.CreateRuntimeLayer(algorithmIncrementWaterDistance1);
            var runtimeIncrementWaterDistance2 = this.CreateRuntimeLayer(algorithmIncrementWaterDistance2);
            var runtimeIncrementWaterDistance3 = this.CreateRuntimeLayer(algorithmIncrementWaterDistance3);
            var runtimeIncrementWaterDistance4 = this.CreateRuntimeLayer(algorithmIncrementWaterDistance4);
            runtimeZoom2DIteration1.SetInput(0, runtimeInitial);
            runtimeIncrementWaterDistance1.SetInput(0, runtimeZoom2DIteration1);
            runtimeZoom2DIteration2.SetInput(0, runtimeIncrementWaterDistance1);
            runtimeIncrementWaterDistance2.SetInput(0, runtimeZoom2DIteration2);
            runtimeZoom2DIteration3.SetInput(0, runtimeIncrementWaterDistance2);
            runtimeIncrementWaterDistance3.SetInput(0, runtimeZoom2DIteration3);
            runtimeZoom2DIteration4.SetInput(0, runtimeIncrementWaterDistance3);
            runtimeIncrementWaterDistance4.SetInput(0, runtimeZoom2DIteration4);
            PerformSampling("AlgorithmIncrementWaterDistance (4 iterations)", runtimeIncrementWaterDistance4);
        }
    }
}
