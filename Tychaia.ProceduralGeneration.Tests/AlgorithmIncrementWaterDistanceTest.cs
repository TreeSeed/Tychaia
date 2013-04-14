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
    public class AlgorithmIncrementWaterDistanceTest
    {
        [Test, TestFor(typeof(AlgorithmIncrementWaterDistance))]
        public void TestNegativeDoubling()
        {
            var inputA = new AlgorithmDebuggingInitialValueDelegate
            {
                GetValueForPosition = (x, y, z, i, j, k) =>
                {
                    return (int)(-1);
                }
            };

            int computations;
            var increment = new AlgorithmIncrementWaterDistance();

            var runtimeInput = new RuntimeLayer(inputA);
            var runtimeTest = new RuntimeLayer(increment);
            runtimeTest.SetInput(0, runtimeInput);

            var result = runtimeTest.GenerateData(0, 0, 0, 16, 16, 16, out computations);


            for (var i = 0; i < 16; i++)
                for (var j = 0; j < 16; j++)
                    for (var k = 0; k < 16; k++)
                    {
                        Assert.IsTrue(
                            result[i + j * 16 + k * 16 * 16] == -2);
                    }
        }        

        [Test, TestFor(typeof(AlgorithmIncrementWaterDistance))]
        public void TestPositiveDoubling()
        {
            var inputA = new AlgorithmDebuggingInitialValueDelegate
            {
                GetValueForPosition = (x, y, z, i, j, k) =>
                {
                    return (int)(1);
                }
            };
            
            int computations;
            var increment = new AlgorithmIncrementWaterDistance();
            
            var runtimeInput = new RuntimeLayer(inputA);
            var runtimeTest = new RuntimeLayer(increment);
            runtimeTest.SetInput(0, runtimeInput);
            
            var result = runtimeTest.GenerateData(0, 0, 0, 16, 16, 16, out computations);
            
            
            for (var i = 0; i < 16; i++)
                for (var j = 0; j < 16; j++)
                    for (var k = 0; k < 16; k++)
                {
                    Assert.IsTrue(
                        result[i + j * 16 + k * 16 * 16] == 2);
                }
        }
    }
}
