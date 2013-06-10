using System;
using Tychaia.ProceduralGeneration.Compiler;
using Xunit;

namespace Tychaia.ProceduralGeneration.Tests
{
    public class CompilerTests
    {
        private void ValidateLayer(RuntimeLayer source)
        {
            var initialBool = new RuntimeLayer(new AlgorithmInitialBool());
            var passthrough = new RuntimeLayer(new AlgorithmPassthrough());
            passthrough.SetInput(0, initialBool);

            var runtime = passthrough;
            var compiledCode = LayerCompiler.GenerateCode(runtime);
            var compiled = LayerCompiler.Compile(runtime);

            int computations;
            var runtimeData = runtime.GenerateData(-10, -10, -10, 20, 20, 20, out computations);
            var compiledData = compiled.GenerateData(-10, -10, -10, 20, 20, 20, out computations);
            var matches = true;
            for (var x = 0; x < 20; x++)
            for (var y = 0; y < 20; y++)
            for (var z = 0; z < 20; z++)
                Assert.Equal(runtimeData[x + y * 20 + z * 20 * 20], compiledData[x + y * 20 + z * 20 * 20]);
        }

        [Fact]
        private void ValidateInitialBoolTest()
        {
            var initialBool = new RuntimeLayer(new AlgorithmInitialBool());
            ValidateLayer(initialBool);
        }

        [Fact]
        private void ValidatePassthroughTest()
        {
            var initialBool = new RuntimeLayer(new AlgorithmInitialBool());
            var passthrough = new RuntimeLayer(new AlgorithmPassthrough());
            passthrough.SetInput(0, initialBool);
            ValidateLayer(passthrough);
        }
    }
}

