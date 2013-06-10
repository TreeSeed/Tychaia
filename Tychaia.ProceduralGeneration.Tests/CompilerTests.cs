using System;
using Tychaia.ProceduralGeneration.Compiler;
using Xunit;

namespace Tychaia.ProceduralGeneration.Tests
{
    public class CompilerTests
    {
        private void ValidateLayer(RuntimeLayer runtime)
        {
            Assert.DoesNotThrow(() =>
            {
                var compiled = LayerCompiler.Compile(runtime);
    
                int computations;
                var runtimeData = runtime.GenerateData(-10, -10, -10, 20, 20, 20, out computations);
                var compiledData = compiled.GenerateData(-10, -10, -10, 20, 20, 20, out computations);
                var matches = true;
                for (var x = 0; x < 20; x++)
                for (var y = 0; y < 20; y++)
                for (var z = 0; z < 20; z++)
                    Assert.Equal(runtimeData[x + y * 20 + z * 20 * 20], compiledData[x + y * 20 + z * 20 * 20]);
            });
        }

        [Fact]
        private void ValidateInitialBoolTest()
        {
            var initialBool = new RuntimeLayer(new AlgorithmInitialBool());
            ValidateLayer(initialBool);
        }

        [Fact]
        private void ValidatePerlinTest()
        {
            var perlin = new RuntimeLayer(new AlgorithmPerlin());
            ValidateLayer(perlin);
        }

        [Fact]
        private void ValidatePassthroughTest()
        {
            var initialBool = new RuntimeLayer(new AlgorithmInitialBool());
            var passthrough = new RuntimeLayer(new AlgorithmPassthrough());
            passthrough.SetInput(0, initialBool);
            ValidateLayer(passthrough);
        }

        [Fact]
        private void ValidateHeightChangeTest()
        {
            var initialBool = new RuntimeLayer(new AlgorithmInitialBool());
            var heightChange = new RuntimeLayer(new AlgorithmHeightChange());
            heightChange.SetInput(0, initialBool);
            ValidateLayer(heightChange);
        }

        [Fact]
        private void ValidateZoom2DTest()
        {
            var initialBool = new RuntimeLayer(new AlgorithmInitialBool());
            var zoom2D = new RuntimeLayer(new AlgorithmZoom2D());
            zoom2D.SetInput(0, initialBool);
            ValidateLayer(zoom2D);
        }

        [Fact]
        private void ValidateZoom2DDoubleTest()
        {
            var initialBool = new RuntimeLayer(new AlgorithmInitialBool());
            var zoom2D1 = new RuntimeLayer(new AlgorithmZoom2D());
            var zoom2D2 = new RuntimeLayer(new AlgorithmZoom2D());
            zoom2D1.SetInput(0, initialBool);
            zoom2D2.SetInput(0, zoom2D1);
            ValidateLayer(zoom2D2);
        }

        [Fact]
        private void ValidateZoom2DTripleTest()
        {
            var initialBool = new RuntimeLayer(new AlgorithmInitialBool());
            var zoom2D1 = new RuntimeLayer(new AlgorithmZoom2D());
            var zoom2D2 = new RuntimeLayer(new AlgorithmZoom2D());
            var zoom2D3 = new RuntimeLayer(new AlgorithmZoom2D());
            zoom2D1.SetInput(0, initialBool);
            zoom2D2.SetInput(0, zoom2D1);
            zoom2D3.SetInput(0, zoom2D2);
            ValidateLayer(zoom2D3);
        }

        [Fact]
        private void ValidateZoom3DTest()
        {
            var perlin = new RuntimeLayer(new AlgorithmPerlin());
            var zoom3D = new RuntimeLayer(new AlgorithmZoom3D());
            zoom3D.SetInput(0, perlin);
            ValidateLayer(zoom3D);
        }

        [Fact]
        private void ValidateZoom3DDoubleTest()
        {
            var perlin = new RuntimeLayer(new AlgorithmPerlin());
            var zoom3D1 = new RuntimeLayer(new AlgorithmZoom3D());
            var zoom3D2 = new RuntimeLayer(new AlgorithmZoom3D());
            zoom3D1.SetInput(0, perlin);
            zoom3D2.SetInput(0, zoom3D1);
            ValidateLayer(zoom3D2);
        }

        [Fact]
        private void ValidateZoom3DTripleTest()
        {
            var perlin = new RuntimeLayer(new AlgorithmPerlin());
            var zoom3D1 = new RuntimeLayer(new AlgorithmZoom3D());
            var zoom3D2 = new RuntimeLayer(new AlgorithmZoom3D());
            var zoom3D3 = new RuntimeLayer(new AlgorithmZoom3D());
            zoom3D1.SetInput(0, perlin);
            zoom3D2.SetInput(0, zoom3D1);
            zoom3D3.SetInput(0, zoom3D2);
            ValidateLayer(zoom3D3);
        }

        [Fact]
        private void ValidateExtend2DTest()
        {
            var initialBool = new RuntimeLayer(new AlgorithmInitialBool());
            var extend2D = new RuntimeLayer(new AlgorithmExtend2D());
            extend2D.SetInput(0, initialBool);
            ValidateLayer(extend2D);
        }
    }
}

