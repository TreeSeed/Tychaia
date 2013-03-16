//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using NUnit.Framework;

namespace Tychaia.ProceduralGeneration.Tests
{
    [TestFixture]
    public class AlgorithmZoomTest
    {
        [Test, TestFor(typeof(AlgorithmZoom2D))]
        public void ZoomSquareTest()
        {
            int computations1, computations2;
            int width = 16, height = 16, depth = 16;
            var gradient = new RuntimeLayer(new AlgorithmGradientInitial());
            var zoom = new RuntimeLayer(new AlgorithmZoom2D { Mode = AlgorithmZoom2D.ZoomType.Square });
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

        [Test, TestFor(typeof(AlgorithmZoom2D))]
        public void TestForCorrectSquareZoomingWithNoOffsetsOrModifications()
        {
            int computations;
            var input = new AlgorithmDebuggingDelegate
            {
                ValueShouldBePlacedAt = (x, y, z) => (x == 4 && y == 6 && z == 7)
            };
            var zoom = new AlgorithmZoom2D
            {
                Mode = AlgorithmZoom2D.ZoomType.Square
            };
            var runtimeInput = new RuntimeLayer(input);
            var runtimeZoom = new RuntimeLayer(zoom);
            runtimeZoom.SetInput(0, runtimeInput);
            var result = runtimeZoom.GenerateData(0, 0, 0, 32, 32, 32, out computations);
            
            // If placed at 4, 6, 7 that will translate to 8, 12, 14 up to 9, 13, 15.
            Assert.IsTrue(result[8 + 12 * 32 + 14 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (original point).");
            Assert.IsTrue(result[9 + 12 * 32 + 14 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (x, 0, 0 zoomed corner).");
            Assert.IsTrue(result[8 + 13 * 32 + 14 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (0, y, 0 zoomed corner).");
            Assert.IsTrue(result[8 + 12 * 32 + 15 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (0, 0, z zoomed corner).");
            Assert.IsTrue(result[9 + 13 * 32 + 14 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (x, y, 0 zoomed corner).");
            Assert.IsTrue(result[8 + 13 * 32 + 15 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (0, y, z zoomed corner).");
            Assert.IsTrue(result[9 + 13 * 32 + 15 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (x, y, z zoomed corner).");
        }
        
        [Test, TestFor(typeof(AlgorithmZoom2D))]
        public void TestForCorrectSquareZoomingWithOffsetX1()
        {
            int computations;
            var input = new AlgorithmDebuggingDelegate
            {
                ValueShouldBePlacedAt = (x, y, z) => (x == 4 && y == 6 && z == 7)
            };
            var zoom = new AlgorithmZoom2D
            {
                Mode = AlgorithmZoom2D.ZoomType.Square
            };
            var runtimeInput = new RuntimeLayer(input);
            var runtimeZoom = new RuntimeLayer(zoom);
            runtimeZoom.SetInput(0, runtimeInput);
            var result = runtimeZoom.GenerateData(1, 0, 0, 32, 32, 32, out computations);

            // Search for the value within the grid to see if it even exists at all.
            var locatedat = "";
            for (var x = 0; x < 32; x++)
                for (var y = 0; y < 32; y++)
                    for (var z = 0; z < 32; z++)
                    {
                        if (result[x + y * 32 + z * 32 * 32] == 1)
                            locatedat += "(" + x + ", " + y + ", " + z + ")";
                    }
            if (locatedat == "")
                locatedat = "nowhere";

            // If placed at 4, 6, 7 that will translate to 7, 12, 14 up to 8, 13, 15.
            Assert.IsTrue(result[7 + 12 * 32 + 14 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (original point).  Found points at " + locatedat + ".");
            Assert.IsTrue(result[8 + 12 * 32 + 14 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (x, 0, 0 zoomed corner).  Found points at " + locatedat + ".");
            Assert.IsTrue(result[7 + 13 * 32 + 14 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (0, y, 0 zoomed corner).  Found points at " + locatedat + ".");
            Assert.IsTrue(result[7 + 12 * 32 + 15 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (0, 0, z zoomed corner).  Found points at " + locatedat + ".");
            Assert.IsTrue(result[8 + 13 * 32 + 14 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (x, y, 0 zoomed corner).  Found points at " + locatedat + ".");
            Assert.IsTrue(result[7 + 13 * 32 + 15 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (0, y, z zoomed corner).  Found points at " + locatedat + ".");
            Assert.IsTrue(result[8 + 13 * 32 + 15 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (x, y, z zoomed corner).  Found points at " + locatedat + ".");
        }
        
        [Test, TestFor(typeof(AlgorithmZoom2D))]
        public void TestForCorrectSquareZoomingWithOffsetX2()
        {
            int computations;
            var input = new AlgorithmDebuggingDelegate
            {
                ValueShouldBePlacedAt = (x, y, z) => (x == 4 && y == 6 && z == 7)
            };
            var zoom = new AlgorithmZoom2D
            {
                Mode = AlgorithmZoom2D.ZoomType.Square
            };
            var runtimeInput = new RuntimeLayer(input);
            var runtimeZoom = new RuntimeLayer(zoom);
            runtimeZoom.SetInput(0, runtimeInput);
            var result = runtimeZoom.GenerateData(2, 0, 0, 32, 32, 32, out computations);
            
            // If placed at 4, 6, 7 that will translate to 6, 12, 14 up to 7, 13, 15.
            Assert.IsTrue(result[6 + 12 * 32 + 14 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (original point).");
            Assert.IsTrue(result[7 + 12 * 32 + 14 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (x, 0, 0 zoomed corner).");
            Assert.IsTrue(result[6 + 13 * 32 + 14 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (0, y, 0 zoomed corner).");
            Assert.IsTrue(result[6 + 12 * 32 + 15 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (0, 0, z zoomed corner).");
            Assert.IsTrue(result[7 + 13 * 32 + 14 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (x, y, 0 zoomed corner).");
            Assert.IsTrue(result[6 + 13 * 32 + 15 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (0, y, z zoomed corner).");
            Assert.IsTrue(result[7 + 13 * 32 + 15 * 32 * 32] == 1, "Square zooming is not working with no offsets or modifications (x, y, z zoomed corner).");
        }
        
        [Test, TestFor(typeof(AlgorithmZoom2D))]
        public void TestForOCXOddAdjustmentShutterBug()
        {
            int computations;
            var input = new AlgorithmDebuggingDelegate
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
                Assert.IsTrue(result[x + 0 * 32 + 0 * 32 * 32] == 1, "OCX odd adjustment shutter bug is present, where every odd row is blank when main adjustment is an odd number.");
        }
    }
}

