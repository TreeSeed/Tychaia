// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Xunit;

namespace Tychaia.ProceduralGeneration.Tests
{
    public class AlgorithmConstantTest : TestBase
    {
        [Fact, TestFor(typeof (AlgorithmConstant))]
        public void TestValues()
        {
            for (var v = -50; v < 50; v++)
            {
                int computations;
                var gradient = this.CreateRuntimeLayer(new AlgorithmConstant { Constant = v });
                var result = gradient.GenerateData(-1, -1, -1, 3, 3, 3, out computations);

                for (var i = 0; i < 3; i++)
                    for (var j = 0; j < 3; j++)
                        for (var k = 0; k < 3; k++)
                            Assert.Equal(result[i + j*3 + k*3*3], v);
            }
        }
    }
}
