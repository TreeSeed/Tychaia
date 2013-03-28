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
    public class BorderTests
    {
        [Test]
        public void TestValueRetrievalAcrossBorder()
        {
            int computations;
            var inputA = new AlgorithmDebuggingInitialDelegate
            {
                ValueShouldBePlacedAt = (x, y, z) => (x >= 4 && x <= 6 && y >= 4 && y <= 6)
            };
            var test = new AlgorithmDebuggingDelegate
            {
                Delegate = (context, input, output, x, y, z, i, j, k, width, height, depth, ox, oy, oz) =>
                {
                    int v00 = input[((i - 1) + ox) + ((j - 1) + oy) * width + (k + oz) * width * height];
                    int v01 = input[((i - 1) + ox) + ((j + 0) + oy) * width + (k + oz) * width * height];
                    int v02 = input[((i - 1) + ox) + ((j + 1) + oy) * width + (k + oz) * width * height];
                    int v10 = input[((i + 0) + ox) + ((j - 1) + oy) * width + (k + oz) * width * height];
                    int v11 = input[((i + 0) + ox) + ((j + 0) + oy) * width + (k + oz) * width * height];
                    int v12 = input[((i + 0) + ox) + ((j + 1) + oy) * width + (k + oz) * width * height];
                    int v20 = input[((i + 1) + ox) + ((j - 1) + oy) * width + (k + oz) * width * height];
                    int v21 = input[((i + 1) + ox) + ((j + 0) + oy) * width + (k + oz) * width * height];
                    int v22 = input[((i + 1) + ox) + ((j + 1) + oy) * width + (k + oz) * width * height];

                    Assert.AreEqual(v00, (5 <= x && x <= 7 && 5 <= y && y <= 7) ? 1 : 0, "v00 != 1 when x == " + x + " && y == " + y + " && i == " + i + " && j == " + j);
                    Assert.AreEqual(v01, (5 <= x && x <= 7 && 4 <= y && y <= 6) ? 1 : 0, "v01 != 1 when x == " + x + " && y == " + y + " && i == " + i + " && j == " + j);
                    Assert.AreEqual(v02, (5 <= x && x <= 7 && 3 <= y && y <= 6) ? 1 : 0, "v02 != 1 when x == " + x + " && y == " + y + " && i == " + i + " && j == " + j);
                    Assert.AreEqual(v10, (4 <= x && x <= 6 && 5 <= y && y <= 7) ? 1 : 0, "v10 != 1 when x == " + x + " && y == " + y + " && i == " + i + " && j == " + j);
                    Assert.AreEqual(v11, (4 <= x && x <= 6 && 4 <= y && y <= 6) ? 1 : 0, "v11 != 1 when x == " + x + " && y == " + y + " && i == " + i + " && j == " + j);
                    Assert.AreEqual(v12, (4 <= x && x <= 6 && 3 <= y && y <= 6) ? 1 : 0, "v12 != 1 when x == " + x + " && y == " + y + " && i == " + i + " && j == " + j);
                    Assert.AreEqual(v20, (3 <= x && x <= 6 && 5 <= y && y <= 7) ? 1 : 0, "v20 != 1 when x == " + x + " && y == " + y + " && i == " + i + " && j == " + j);
                    Assert.AreEqual(v21, (3 <= x && x <= 6 && 4 <= y && y <= 6) ? 1 : 0, "v21 != 1 when x == " + x + " && y == " + y + " && i == " + i + " && j == " + j);
                    Assert.AreEqual(v22, (3 <= x && x <= 6 && 3 <= y && y <= 6) ? 1 : 0, "v22 != 1 when x == " + x + " && y == " + y + " && i == " + i + " && j == " + j);
                }
            };
            var runtimeInput = new RuntimeLayer(inputA);
            var runtimeTest = new RuntimeLayer(test);
            runtimeTest.SetInput(0, runtimeInput);

            // Test this stuff.
            for (int i = -10; i < 10; i++)
            {
                runtimeTest.GenerateData(i, i, i, 10, 10, 10, out computations);
            }
        }
    }
}

