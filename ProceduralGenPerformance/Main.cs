//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Compiler;

namespace ProceduralGenPerformance
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // Create an initial land layer from both types.
            var initialLandLegacy = new LayerInitialLand();
            var initialLandAlgorithmRuntime = new RuntimeLayer(new AlgorithmInitialLand());
            var initialLandAlgorithmCompiled = LayerCompiler.Compile(initialLandAlgorithmRuntime);

            // Run tests to see how fast they are.
            for (int x = 0; x < 300; x++)
            {
                var legacyStartTime = DateTime.Now;
                Console.WriteLine("Starting Test #" + x + " (legacy)");
                for (int i = 0; i < 1000; i++)
                    initialLandLegacy.GenerateData(0, 0, 128, 128);
                var legacyEndTime = DateTime.Now;
                Console.WriteLine("Starting Test #" + x + " (algorithm runtime)");
                var algorithmRuntimeStartTime = DateTime.Now;
                for (int i = 0; i < 1000; i++)
                    initialLandAlgorithmRuntime.GenerateData(0, 0, 0, 128, 128, 1);
                var algorithmRuntimeEndTime = DateTime.Now;
                Console.WriteLine("Starting Test #" + x + " (algorithm compiled)");
                var algorithmCompiledStartTime = DateTime.Now;
                for (int i = 0; i < 1000; i++)
                    initialLandAlgorithmCompiled.GenerateData(0, 0, 0, 128, 128, 1);
                var algorithmCompiledEndTime = DateTime.Now;
                // Because there are 1000 tests, and 1000 microseconds in a millisecond..
                Console.WriteLine("Test #" + x + " LEGACY: " + (legacyEndTime - legacyStartTime).TotalMilliseconds + "µs " + 
                    "ALGORITHM RUNTIME: " + (algorithmRuntimeEndTime - algorithmRuntimeStartTime).TotalMilliseconds + "µs " +
                    "ALGORITHM COMPILED: " + (algorithmCompiledEndTime - algorithmCompiledStartTime).TotalMilliseconds + "µs");
            }
        }
    }
}
