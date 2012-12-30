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
            Layer legacy;
            RuntimeLayer algorithmRuntime;
            IGenerator algorithmCompiled;
            string mode = "quadzoom";
            if (args.Length > 0)
                mode = args[0];

            // Create initial layers from both types.
            if (mode == "quadzoom")
            {
                legacy = new LayerZoom(new LayerZoom(new LayerZoom(new LayerZoom(new LayerInitialLand()))));
                var algorithmZoom1 = new RuntimeLayer(new AlgorithmZoom());
                var algorithmZoom2 = new RuntimeLayer(new AlgorithmZoom());
                var algorithmZoom3 = new RuntimeLayer(new AlgorithmZoom());
                var algorithmZoom4 = new RuntimeLayer(new AlgorithmZoom());
                var algorithmInitialLand = new RuntimeLayer(new AlgorithmInitialLand());
                algorithmZoom4.SetInput(0, algorithmInitialLand);
                algorithmZoom3.SetInput(0, algorithmZoom4);
                algorithmZoom2.SetInput(0, algorithmZoom3);
                algorithmZoom1.SetInput(0, algorithmZoom2);
                algorithmRuntime = algorithmZoom1;
                algorithmCompiled = LayerCompiler.Compile(algorithmRuntime);
            }
            else if (mode == "doublezoom")
            {
                legacy = new LayerZoom(new LayerZoom(new LayerInitialLand()));
                var algorithmZoom1 = new RuntimeLayer(new AlgorithmZoom());
                var algorithmZoom2 = new RuntimeLayer(new AlgorithmZoom());
                var algorithmInitialLand = new RuntimeLayer(new AlgorithmInitialLand());
                algorithmZoom2.SetInput(0, algorithmInitialLand);
                algorithmZoom1.SetInput(0, algorithmZoom2);
                algorithmRuntime = algorithmZoom1;
                algorithmCompiled = LayerCompiler.Compile(algorithmRuntime);
            }
            else if (mode == "zoom")
            {
                legacy = new LayerZoom(new LayerInitialLand());
                algorithmRuntime = new RuntimeLayer(new AlgorithmZoom());
                algorithmRuntime.SetInput(0, new RuntimeLayer(new AlgorithmInitialLand()));
                algorithmCompiled = LayerCompiler.Compile(algorithmRuntime);
            }
            else if (mode == "land")
            {
                legacy = new LayerInitialLand();
                algorithmRuntime = new RuntimeLayer(new AlgorithmInitialLand());
                algorithmCompiled = LayerCompiler.Compile(algorithmRuntime);
            }
            else
            {
                Console.WriteLine("usage: ProceduralGenPerformance.exe [doublezoom|zoom|land]");
                return;
            }

            // Run tests to see how fast they are.
            for (int x = 0; x < 300; x++)
            {
                var legacyStartTime = DateTime.Now;
                Console.WriteLine("Starting Test #" + x + " (legacy)");
                for (int i = 0; i < 1000; i++)
                    legacy.GenerateData(0, 0, 128, 128);
                var legacyEndTime = DateTime.Now;
                Console.WriteLine("Starting Test #" + x + " (algorithm runtime)");
                var algorithmRuntimeStartTime = DateTime.Now;
                for (int i = 0; i < 1000; i++)
                    algorithmRuntime.GenerateData(0, 0, 0, 128, 128, 1);
                var algorithmRuntimeEndTime = DateTime.Now;
                Console.WriteLine("Starting Test #" + x + " (algorithm compiled)");
                var algorithmCompiledStartTime = DateTime.Now;
                for (int i = 0; i < 1000; i++)
                    algorithmCompiled.GenerateData(0, 0, 0, 128, 128, 1);
                var algorithmCompiledEndTime = DateTime.Now;
                // Because there are 1000 tests, and 1000 microseconds in a millisecond..
                Console.WriteLine(
                    "Test #" + x + " "
                    + "LEGACY: " + (legacyEndTime - legacyStartTime).TotalMilliseconds + "µs "
                    + "ALGORITHM RUNTIME: " + (algorithmRuntimeEndTime - algorithmRuntimeStartTime).TotalMilliseconds + "µs "
                    + "ALGORITHM COMPILED: " + (algorithmCompiledEndTime - algorithmCompiledStartTime).TotalMilliseconds + "µs "
                    + "LC%: " + ((legacyEndTime - legacyStartTime).TotalMilliseconds / (algorithmCompiledEndTime - algorithmCompiledStartTime).TotalMilliseconds) * 100 + "% "
                );
            }
        }
    }
}
