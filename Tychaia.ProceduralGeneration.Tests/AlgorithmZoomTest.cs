//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using NUnit.Framework;

namespace Tychaia.ProceduralGeneration.Tests
{
    [TestFixture()]
    public class AlgorithmZoomTest
    {
        [Test()]
        public void ZoomSquareTest()
        {
            int computations1, computations2;
            int width = 16, height = 16, depth = 16;
            var gradient = new RuntimeLayer(new AlgorithmGradientInitial());
            var zoom = new RuntimeLayer(new AlgorithmZoom { Mode = AlgorithmZoom.ZoomType.Square });
            zoom.SetInput(0, gradient);
            var i1 = gradient.GenerateData(0, 0, 0, width, height, depth, out computations1);
            var i2 = zoom.GenerateData(0, 0, 0, width, height, depth, out computations2);
            
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    for (var k = 0; k < depth; k++)
                        Assert.AreEqual(
                            i1[i / 2 + j / 2 * width + k / 2 * width * height],
                            i2[i + j * width + k * width * height],
                            "Square zoom is not working for (" + i + ", " + j + ", " + k + ").");
        }
    }
}

