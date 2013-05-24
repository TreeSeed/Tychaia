//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Xunit;

namespace Tychaia.ProceduralGeneration.Tests
{
    
    public class AlgorithmInitialBoolTest
    {
        [Fact, TestFor(typeof(AlgorithmInitialBool))]
        public void TestRange()
        {
            int computations;
            var gradient = new RuntimeLayer(new AlgorithmInitialBool());
            var result = gradient.GenerateData(0, 0, 0, 16, 16, 16, out computations);
            
            for (var i = 0; i < 16; i++)
                for (var j = 0; j < 16; j++)
                    for (var k = 0; k < 16; k++)
                    {
                        if (result[i + j * 16 + k * 16 * 16] == 0)
                            continue;

                        Assert.True(
                            result[i + j * 16 + k * 16 * 16] == 0 || 
                            result[i + j * 16 + k * 16 * 16] == 1);
                    }
        }
    }
}

