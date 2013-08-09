// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
namespace Tychaia.ProceduralGeneration.Tests
{
    // These tests are disabled until the compiler implementation is fixed to match
    // the runtime implementation.

#if DISABLED


    public class CompilerTest
    {
        [Fact]
        public void TestCompileGradient()
        {
            int computations1, computations2;
            int width = 16, height = 16, depth = 16;
            var gradient = new RuntimeLayer(new AlgorithmGradientInitial());
            var passthrough = new RuntimeLayer(new AlgorithmPassthrough());
            passthrough.SetInput(0, gradient);
            var i1 = gradient.GenerateData(0, 0, 0, width, height, depth, out computations1);

            var c1 = LayerCompiler.Compile(gradient).GenerateData(0, 0, 0, width, height, depth, out computations1);

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    for (var z = 0; z < depth; z++)
                        Assert.Equal(
                            i1[x + y * width + z * width * height],
                            c1[x + y * width + z * width * height],
                            "Value differs in gradient compare.");
        }

        [Fact]
        public void TestCompilePassthrough()
        {
            int computations1, computations2;
            int width = 16, height = 16, depth = 16;
            var gradient = new RuntimeLayer(new AlgorithmGradientInitial());
            var passthrough = new RuntimeLayer(new AlgorithmPassthrough());
            passthrough.SetInput(0, gradient);
            var i1 = gradient.GenerateData(0, 0, 0, width, height, depth, out computations1);
            var i2 = passthrough.GenerateData(0, 0, 0, width, height, depth, out computations2);

            var c1 = LayerCompiler.Compile(gradient).GenerateData(0, 0, 0, width, height, depth, out computations1);
            var c2 = LayerCompiler.Compile(passthrough).GenerateData(0, 0, 0, width, height, depth, out computations2);

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    for (var z = 0; z < depth; z++)
                        Assert.Equal(
                            i1[x + y * width + z * width * height],
                            c1[x + y * width + z * width * height],
                            "Value differs in gradient compare.");

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    for (var z = 0; z < depth; z++)
                        Assert.Equal(
                            i2[x + y * width + z * width * height],
                            c2[x + y * width + z * width * height],
                            "Value differs in passthrough compare.");
        }
    }

    #endif
}