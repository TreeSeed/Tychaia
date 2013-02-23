//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using NUnit.Framework;
using Tychaia.ProceduralGeneration.Compiler;

namespace Tychaia.ProceduralGeneration.Tests
{
    [TestFixture()]
    public class CompilerTest
    {
        [Test()]
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
                        Assert.AreEqual(
                            i1[x + y * width + z * width * height],
                            c1[x + y * width + z * width * height],
                            "Value differs in gradient compare.");
        }

        [Test()]
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
                        Assert.AreEqual(
                            i1[x + y * width + z * width * height],
                            c1[x + y * width + z * width * height],
                            "Value differs in gradient compare.");
            
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    for (var z = 0; z < depth; z++)
                        Assert.AreEqual(
                            i2[x + y * width + z * width * height],
                            c2[x + y * width + z * width * height],
                            "Value differs in passthrough compare.");
        }
    }
}

