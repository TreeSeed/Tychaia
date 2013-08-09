// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using Xunit;

namespace Tychaia.ProceduralGeneration.Tests
{
    public class SkewTests
    {
        [Fact]
        public void TestXSkew()
        {
            int computations;
            var input = new AlgorithmDebuggingInitialDelegate
            {
                ValueShouldBePlacedAt = (x, y, z) => (y == 0 && z == 0)
            };
            var passthrough = new AlgorithmPassthrough();
            var runtimeInput = new RuntimeLayer(input);
            var runtimePassthough = new RuntimeLayer(passthrough);
            runtimePassthough.SetInput(0, runtimeInput);

            // We need to check with various borders.
            for (var i = 0; i < 2; i++)
                for (var j = 0; j < 2; j++)
                    for (var k = 0; k < 2; k++)
                    {
                        passthrough.XBorder = i;
                        passthrough.YBorder = j;
                        passthrough.ZBorder = k;
                        var result = runtimePassthough.GenerateData(0, -16, -16, 32, 32, 32, out computations);

                        // Test the area where we should be filled.
                        for (var x = 0; x < 32; x += 1)
                            Assert.True(result[x + 16*32 + 16*32*32] == 1,
                                "Skew present on the X axis with borders (" + i + ", " + j + ", " + k +
                                "), value missing.");

                        // Test the areas where we should not be filled.
                        for (var x = 0; x < 32; x += 1)
                            Assert.False(result[x + 17*32 + 15*32*32] == 1,
                                "Skew present on the X axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (" + x + ", 1, -1).");
                        for (var x = 0; x < 32; x += 1)
                            Assert.False(result[x + 16*32 + 15*32*32] == 1,
                                "Skew present on the X axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (" + x + ", 0, -1).");
                        for (var x = 0; x < 32; x += 1)
                            Assert.False(result[x + 15*32 + 15*32*32] == 1,
                                "Skew present on the X axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (" + x + ", -1, -1).");
                        for (var x = 0; x < 32; x += 1)
                            Assert.False(result[x + 17*32 + 16*32*32] == 1,
                                "Skew present on the X axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (" + x + ", 1, 0).");
                        for (var x = 0; x < 32; x += 1)
                            Assert.False(result[x + 15*32 + 16*32*32] == 1,
                                "Skew present on the X axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (" + x + ", -1, 0).");
                        for (var x = 0; x < 32; x += 1)
                            Assert.False(result[x + 17*32 + 17*32*32] == 1,
                                "Skew present on the X axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (" + x + ", 1, 1).");
                        for (var x = 0; x < 32; x += 1)
                            Assert.False(result[x + 16*32 + 17*32*32] == 1,
                                "Skew present on the X axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (" + x + ", 0, 1).");
                        for (var x = 0; x < 32; x += 1)
                            Assert.False(result[x + 15*32 + 17*32*32] == 1,
                                "Skew present on the X axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (" + x + ", -1, 1).");
                    }
        }

        [Fact]
        public void TestYSkew()
        {
            int computations;
            var input = new AlgorithmDebuggingInitialDelegate
            {
                ValueShouldBePlacedAt = (x, y, z) => (x == 0 && z == 0)
            };
            var passthrough = new AlgorithmPassthrough();
            var runtimeInput = new RuntimeLayer(input);
            var runtimePassthough = new RuntimeLayer(passthrough);
            runtimePassthough.SetInput(0, runtimeInput);

            // We need to check with various borders.
            for (var i = 0; i < 2; i++)
                for (var j = 0; j < 2; j++)
                    for (var k = 0; k < 2; k++)
                    {
                        passthrough.XBorder = i;
                        passthrough.YBorder = j;
                        passthrough.ZBorder = k;
                        var result = runtimePassthough.GenerateData(-16, 0, -16, 32, 32, 32, out computations);

                        // Test the area where we should be filled.
                        for (var y = 0; y < 32; y += 1)
                            Assert.True(result[16 + y*32 + 16*32*32] == 1,
                                "Skew present on the Y axis with borders (" + i + ", " + j + ", " + k +
                                "), value missing.");

                        // Test the areas where we should not be filled.
                        for (var y = 0; y < 32; y += 1)
                            Assert.False(result[17 + y*32 + 15*32*32] == 1,
                                "Skew present on the Y axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (1, " + y + ", -1).");
                        for (var y = 0; y < 32; y += 1)
                            Assert.False(result[16 + y*32 + 15*32*32] == 1,
                                "Skew present on the Y axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (0, " + y + ", -1).");
                        for (var y = 0; y < 32; y += 1)
                            Assert.False(result[15 + y*32 + 15*32*32] == 1,
                                "Skew present on the Y axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (-1, " + y + ", -1).");
                        for (var y = 0; y < 32; y += 1)
                            Assert.False(result[17 + y*32 + 16*32*32] == 1,
                                "Skew present on the Y axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (1, " + y + ", 0).");
                        for (var y = 0; y < 32; y += 1)
                            Assert.False(result[15 + y*32 + 16*32*32] == 1,
                                "Skew present on the Y axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (-1, " + y + ", 0).");
                        for (var y = 0; y < 32; y += 1)
                            Assert.False(result[17 + y*32 + 17*32*32] == 1,
                                "Skew present on the Y axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (1, " + y + ", 1).");
                        for (var y = 0; y < 32; y += 1)
                            Assert.False(result[16 + y*32 + 17*32*32] == 1,
                                "Skew present on the Y axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (0, " + y + ", 1).");
                        for (var y = 0; y < 32; y += 1)
                            Assert.False(result[15 + y*32 + 17*32*32] == 1,
                                "Skew present on the Y axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (-1, " + y + ", 1).");
                    }
        }

        [Fact]
        public void TestZSkew()
        {
            int computations;
            var input = new AlgorithmDebuggingInitialDelegate
            {
                ValueShouldBePlacedAt = (x, y, z) => (x == 0 && y == 0)
            };
            var passthrough = new AlgorithmPassthrough();
            var runtimeInput = new RuntimeLayer(input);
            var runtimePassthough = new RuntimeLayer(passthrough);
            runtimePassthough.SetInput(0, runtimeInput);

            // We need to check with various borders.
            for (var i = 0; i < 2; i++)
                for (var j = 0; j < 2; j++)
                    for (var k = 0; k < 2; k++)
                    {
                        passthrough.XBorder = i;
                        passthrough.YBorder = j;
                        passthrough.ZBorder = k;
                        var result = runtimePassthough.GenerateData(-16, -16, 0, 32, 32, 32, out computations);

                        // Test the area where we should be filled.
                        for (var z = 0; z < 32; z += 1)
                            Assert.True(result[16 + 16*32 + z*32*32] == 1,
                                "Skew present on the Z axis with borders (" + i + ", " + j + ", " + k +
                                "), value missing.");

                        // Test the areas where we should not be filled.
                        for (var z = 0; z < 32; z += 1)
                            Assert.False(result[15 + 17*32 + z*32*32] == 1,
                                "Skew present on the Z axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (-1, 1, " + z + ").");
                        for (var z = 0; z < 32; z += 1)
                            Assert.False(result[15 + 16*32 + z*32*32] == 1,
                                "Skew present on the Z axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (-1, 0, " + z + ").");
                        for (var z = 0; z < 32; z += 1)
                            Assert.False(result[15 + 15*32 + z*32*32] == 1,
                                "Skew present on the Z axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (-1, -1, " + z + ").");
                        for (var z = 0; z < 32; z += 1)
                            Assert.False(result[16 + 17*32 + z*32*32] == 1,
                                "Skew present on the Z axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (0, 1, " + z + ").");
                        for (var z = 0; z < 32; z += 1)
                            Assert.False(result[16 + 15*32 + z*32*32] == 1,
                                "Skew present on the Z axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (0, -1, " + z + ").");
                        for (var z = 0; z < 32; z += 1)
                            Assert.False(result[17 + 17*32 + z*32*32] == 1,
                                "Skew present on the Z axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (1, 1, " + z + ").");
                        for (var z = 0; z < 32; z += 1)
                            Assert.False(result[17 + 16*32 + z*32*32] == 1,
                                "Skew present on the Z axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (1, 0, " + z + ").");
                        for (var z = 0; z < 32; z += 1)
                            Assert.False(result[17 + 15*32 + z*32*32] == 1,
                                "Skew present on the Z axis with borders (" + i + ", " + j + ", " + k +
                                "), value present at (1, -1, " + z + ").");
                    }
        }
    }
}