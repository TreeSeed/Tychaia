// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Tychaia.ProceduralGeneration.Compiler;
using Xunit;

namespace Tychaia.ProceduralGeneration.Tests
{
    public class CompilerTests : TestBase
    {
        private void ValidateLayer(RuntimeLayer runtime)
        {
            Assert.DoesNotThrow(() =>
            {
                var compiled = LayerCompiler.Compile(runtime);

                int computations;
                var runtimeData = runtime.GenerateData(-10, -10, -10, 20, 20, 20, out computations);
                var compiledData = compiled.GenerateData(-10, -10, -10, 20, 20, 20, out computations);
                for (var x = 0; x < 20; x++)
                    for (var y = 0; y < 20; y++)
                        for (var z = 0; z < 20; z++)
                            Assert.Equal(runtimeData[x + y*20 + z*20*20], compiledData[x + y*20 + z*20*20]);
            });
        }

        [Fact(Skip = "Compiler is unstable")]
        private void ValidateInitialBool()
        {
            var initialBool = this.CreateRuntimeLayer(new AlgorithmInitialBool());
            this.ValidateLayer(initialBool);
        }

        [Fact(Skip = "Compiler is unstable")]
        private void ValidatePerlin()
        {
            var perlin = this.CreateRuntimeLayer(new AlgorithmPerlin());
            this.ValidateLayer(perlin);
        }

        [Fact(Skip = "Compiler is unstable")]
        private void ValidatePassthrough()
        {
            var initialBool = this.CreateRuntimeLayer(new AlgorithmInitialBool());
            var passthrough = this.CreateRuntimeLayer(new AlgorithmPassthrough());
            passthrough.SetInput(0, initialBool);
            this.ValidateLayer(passthrough);
        }

        [Fact(Skip = "Compiler is unstable")]
        private void ValidateHeightChange()
        {
            var initialBool = this.CreateRuntimeLayer(new AlgorithmInitialBool());
            var heightChange = this.CreateRuntimeLayer(new AlgorithmHeightChange());
            heightChange.SetInput(0, initialBool);
            this.ValidateLayer(heightChange);
        }

        [Fact(Skip = "Compiler not yet fully working")]
        private void ValidateZoom2D()
        {
            var initialBool = this.CreateRuntimeLayer(new AlgorithmInitialBool());
            var zoom2D = this.CreateRuntimeLayer(new AlgorithmZoom2D());
            zoom2D.SetInput(0, initialBool);
            this.ValidateLayer(zoom2D);
        }

        [Fact(Skip = "Compiler not yet fully working")]
        private void ValidateZoom2DDouble()
        {
            var initialBool = this.CreateRuntimeLayer(new AlgorithmInitialBool());
            var zoom2D1 = this.CreateRuntimeLayer(new AlgorithmZoom2D());
            var zoom2D2 = this.CreateRuntimeLayer(new AlgorithmZoom2D());
            zoom2D1.SetInput(0, initialBool);
            zoom2D2.SetInput(0, zoom2D1);
            this.ValidateLayer(zoom2D2);
        }

        [Fact(Skip = "Compiler not yet fully working")]
        private void ValidateZoom2DTriple()
        {
            var initialBool = this.CreateRuntimeLayer(new AlgorithmInitialBool());
            var zoom2D1 = this.CreateRuntimeLayer(new AlgorithmZoom2D());
            var zoom2D2 = this.CreateRuntimeLayer(new AlgorithmZoom2D());
            var zoom2D3 = this.CreateRuntimeLayer(new AlgorithmZoom2D());
            zoom2D1.SetInput(0, initialBool);
            zoom2D2.SetInput(0, zoom2D1);
            zoom2D3.SetInput(0, zoom2D2);
            this.ValidateLayer(zoom2D3);
        }

        [Fact(Skip = "Compiler not yet fully working")]
        private void ValidateZoom3D()
        {
            var perlin = this.CreateRuntimeLayer(new AlgorithmPerlin());
            var zoom3D = this.CreateRuntimeLayer(new AlgorithmZoom3D());
            zoom3D.SetInput(0, perlin);
            this.ValidateLayer(zoom3D);
        }

        [Fact(Skip = "Compiler not yet fully working")]
        private void ValidateZoom3DDouble()
        {
            var perlin = this.CreateRuntimeLayer(new AlgorithmPerlin());
            var zoom3D1 = this.CreateRuntimeLayer(new AlgorithmZoom3D());
            var zoom3D2 = this.CreateRuntimeLayer(new AlgorithmZoom3D());
            zoom3D1.SetInput(0, perlin);
            zoom3D2.SetInput(0, zoom3D1);
            this.ValidateLayer(zoom3D2);
        }

        [Fact(Skip = "Compiler not yet fully working")]
        private void ValidateZoom3DTriple()
        {
            var perlin = this.CreateRuntimeLayer(new AlgorithmPerlin());
            var zoom3D1 = this.CreateRuntimeLayer(new AlgorithmZoom3D());
            var zoom3D2 = this.CreateRuntimeLayer(new AlgorithmZoom3D());
            var zoom3D3 = this.CreateRuntimeLayer(new AlgorithmZoom3D());
            zoom3D1.SetInput(0, perlin);
            zoom3D2.SetInput(0, zoom3D1);
            zoom3D3.SetInput(0, zoom3D2);
            this.ValidateLayer(zoom3D3);
        }

        [Fact(Skip = "Compiler is unstable")]
        private void ValidateExtend2D()
        {
            var initialBool = this.CreateRuntimeLayer(new AlgorithmInitialBool());
            var extend2D = this.CreateRuntimeLayer(new AlgorithmExtend2D());
            extend2D.SetInput(0, initialBool);
            this.ValidateLayer(extend2D);
        }

        [Fact(Skip = "Compiler is unstable")]
        private void ValidateComplexStructureWithoutZooms()
        {
            var perlin = this.CreateRuntimeLayer(new AlgorithmPerlin());
            var add = this.CreateRuntimeLayer(new AlgorithmAdd());
            var perlin2 = this.CreateRuntimeLayer(new AlgorithmPerlin());
            var passthrough = this.CreateRuntimeLayer(new AlgorithmPassthrough {XBorder = 7, YBorder = 9, ZBorder = 11});
            var heightC = this.CreateRuntimeLayer(new AlgorithmHeightChange());
            passthrough.SetInput(0, perlin2);
            add.SetInput(0, perlin);
            add.SetInput(1, passthrough);
            heightC.SetInput(0, add);
            this.ValidateLayer(heightC);
        }
        
        [Fact(Skip = "Compiler not yet fully working")]
        public void TestCompileGradient()
        {
            int computations1;
            int width = 16, height = 16, depth = 16;
            var gradient = this.CreateRuntimeLayer(new AlgorithmGradientInitial());
            var passthrough = this.CreateRuntimeLayer(new AlgorithmPassthrough());
            passthrough.SetInput(0, gradient);
            var i1 = gradient.GenerateData(0, 0, 0, width, height, depth, out computations1);

            var c1 = LayerCompiler.Compile(gradient).GenerateData(0, 0, 0, width, height, depth, out computations1);

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    for (var z = 0; z < depth; z++)
                        Assert.Equal(
                            i1[x + y * width + z * width * height],
                            c1[x + y * width + z * width * height]);
        }

        [Fact(Skip = "Compiler not yet fully working")]
        public void TestCompilePassthrough()
        {
            int computations1, computations2;
            int width = 16, height = 16, depth = 16;
            var gradient = this.CreateRuntimeLayer(new AlgorithmGradientInitial());
            var passthrough = this.CreateRuntimeLayer(new AlgorithmPassthrough());
            passthrough.SetInput(0, gradient);
            var i1 = gradient.GenerateData(0, 0, 0, width, height, depth, out computations1);
            var i2 = passthrough.GenerateData(0, 0, 0, width, height, depth, out computations2);

            var c1 = this.CompileLayer(gradient).GenerateData(0, 0, 0, width, height, depth, out computations1);
            var c2 = this.CompileLayer(passthrough).GenerateData(0, 0, 0, width, height, depth, out computations2);

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    for (var z = 0; z < depth; z++)
                        Assert.Equal(
                            i1[x + y * width + z * width * height],
                            c1[x + y * width + z * width * height]);

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    for (var z = 0; z < depth; z++)
                        Assert.Equal(
                            i2[x + y * width + z * width * height],
                            c2[x + y * width + z * width * height]);
        }
    }
}
