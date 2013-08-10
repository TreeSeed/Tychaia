// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using Xunit;

namespace Tychaia.ProceduralGeneration.Tests
{
    public class AlgorithmZoomTest
    {
        [Fact, TestFor(typeof (AlgorithmZoom2D))]
        public void ZoomSquareTest()
        {
            int computations1, computations2;
            int width = 16, height = 16, depth = 16;
            var gradient = new RuntimeLayer(new AlgorithmGradientInitial());
            var zoom = new RuntimeLayer(new AlgorithmZoom2D {Mode = AlgorithmZoom2D.ZoomType.Square});
            zoom.SetInput(0, gradient);
            var i1 = gradient.GenerateData(0, 0, 0, width, height, depth, out computations1);
            var i2 = zoom.GenerateData(0, 0, 0, width, height, depth, out computations2);

            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    for (var k = 0; k < depth; k++)
                        Assert.Equal(
                            i1[i/2 + j/2*width + k/2*width*height],
                            i2[i + j*width + k*width*height]);
            //,
            //               "Square zoom is not working for (" + i + ", " + j + ", " + k + ").");
        }

        [Fact, TestFor(typeof (AlgorithmZoom2D))]
        public void TestForOCXOddAdjustmentShutterBug()
        {
            int computations;
            var input = new AlgorithmDebuggingInitialDelegate
            {
                ValueShouldBePlacedAt = (x, y, z) => true
            };
            var zoom = new AlgorithmZoom2D
            {
                Mode = AlgorithmZoom2D.ZoomType.Square
            };
            var runtimeInput = new RuntimeLayer(input);
            var runtimeZoom = new RuntimeLayer(zoom);
            runtimeZoom.SetInput(0, runtimeInput);
            var result = runtimeZoom.GenerateData(1, 0, 0, 32, 32, 32, out computations);

            // We have filled the entire block, therefore this bug can be detected by checking
            // every odd row.
            for (var x = 1; x < 32; x += 2)
                Assert.True(result[x + 0*32 + 0*32*32] == 1);
            //, "OCX odd adjustment shutter bug is present, where every odd row is blank when main adjustment is an odd number.");
        }
    }
}