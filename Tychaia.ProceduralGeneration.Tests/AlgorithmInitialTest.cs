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
    public class AlgorithmInitialTest
    {
        [Test, TestFor(typeof(AlgorithmInitial))]
        public void TestRange()
        {
            int computations;
            var minimum = 39;
            var maximum = 67;
            var gradient = new RuntimeLayer(new AlgorithmInitial { MinimumValue = minimum, MaximumValue = maximum });
            var result = gradient.GenerateData(0, 0, 0, 16, 16, 16, out computations);
            
            for (var i = 0; i < 16; i++)
                for (var j = 0; j < 16; j++)
                    for (var k = 0; k < 16; k++)
                    {
                        if (result[i + j * 16 + k * 16 * 16] == 0)
                            continue;

                        Assert.GreaterOrEqual(result[i + j * 16 + k * 16 * 16], minimum);
                        Assert.LessOrEqual(result[i + j * 16 + k * 16 * 16], maximum);
                    }
        }
    }
}

