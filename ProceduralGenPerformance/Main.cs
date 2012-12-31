//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

//#define MODULO_SPEED_TEST
//#define VERIFICATION

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
            IGenerator algorithmCompiledBuiltin = null;
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
                algorithmCompiledBuiltin = new CompiledLayerBuiltin();
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

#if MODULO_SPEED_TEST

            for (int i = 0; i <= 10000000; i += 1)
            {
                if ((i & 71) < 0 || (i & 71) >= 10000000)
                    Console.WriteLine("AND DOES NOT WORK FOR " + i + ".");
            }
            int a = 0;
            var andStartTime = DateTime.Now;
            for (int i = 0; i < 10000000; i += 1)
            {
                a = i & 71;
            }
            var andEndTime = DateTime.Now;
            var modStartTime = DateTime.Now;
            for (int i = 0; i < 10000000; i += 1)
            {
                a = i % 71;
            }
            var modEndTime = DateTime.Now;
            Console.WriteLine("BITAND OPERATION TOOK: " + (andEndTime - andStartTime).TotalMilliseconds + "ms");
            Console.WriteLine("MODULO OPERATION TOOK: " + (modEndTime - modStartTime).TotalMilliseconds + "ms");

#endif

#if VERIFICATION

            #region Verification

            // Verify integrity between algorithms.
            int[] legacyData = legacy.GenerateData(0, 0, 128, 128);
            int[] runtimeData = algorithmRuntime.GenerateData(0, 0, 0, 128, 128, 1);
            int[] compiledData = algorithmCompiled.GenerateData(0, 0, 0, 128, 128, 1);
            if (runtimeData.Length != legacyData.Length)
            {
                Console.WriteLine("STOP: Runtime data evaluation results in a different array size than the legacy system.");
                return;
            }
            if (compiledData.Length != legacyData.Length)
            {
                Console.WriteLine("STOP: Runtime data evaluation results in a different array size than the legacy system.");
                return;
            }
            for (var i = 0; i < legacyData.Length; i++)
            {
                if (runtimeData[i] != legacyData[i])
                {
                    Console.WriteLine("WARNING: Runtime algorithm results in different data to legacy system.");
                    break;
                }
            }
            for (var i = 0; i < legacyData.Length; i++)
            {
                if (compiledData[i] != legacyData[i])
                {
                    Console.WriteLine("WARNING: Runtime algorithm results in different data to legacy system.");
                    break;
                }
            }
            for (var i = 0; i < legacyData.Length; i++)
            {
                if (runtimeData[i] != compiledData[i])
                {
                    Console.WriteLine("STOP: Runtime algorithm results in different data to compiled algorithm.");
                    for (var x = 0; x < 128; x++)
                    {
                        for (var y = 0; y < 128; y++)
                        {
                            if (runtimeData[x + y * 128] != compiledData[x + y * 128])
                                Console.Write(compiledData[x + y * 128]);
                            else
                                Console.Write(" ");
                        }
                        Console.WriteLine();
                    }
                    break;
                }
            }
            Console.WriteLine("=== SAMPLE FROM ALGORITHM ===");
            for (var x = 0; x < 128; x++)
            {
                for (var y = 0; y < 128; y++)
                {
                    Console.Write(runtimeData[x + y * 128]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("=============================");

            #endregion

#endif

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
                var algorithmCompiledBuiltinStartTime = DateTime.Now;
                var algorithmCompiledBuiltinEndTime = DateTime.Now;
                if (algorithmCompiledBuiltin != null)
                {
                    Console.WriteLine("Starting Test #" + x + " (algorithm compiled builtin)");
                    algorithmCompiledBuiltinStartTime = DateTime.Now;
                    for (int i = 0; i < 1000; i++)
                        algorithmCompiled.GenerateData(0, 0, 0, 128, 128, 1);
                    algorithmCompiledBuiltinEndTime = DateTime.Now;
                }
                // Because there are 1000 tests, and 1000 microseconds in a millisecond..
                Console.WriteLine(
                    "Test #" + x + " "
                    + "LEGACY: " + (legacyEndTime - legacyStartTime).TotalMilliseconds + "µs "
                    + "ALGORITHM RUNTIME: " + (algorithmRuntimeEndTime - algorithmRuntimeStartTime).TotalMilliseconds + "µs "
                    + "ALGORITHM COMPILED: " + (algorithmCompiledEndTime - algorithmCompiledStartTime).TotalMilliseconds + "µs "
                    + ((algorithmCompiledBuiltin != null) ? ("ALGORITHM COMPILED BUILTIN: " + (algorithmCompiledBuiltinEndTime - algorithmCompiledBuiltinStartTime).TotalMilliseconds + "µs ") : "")
                    + "LC%: " + ((legacyEndTime - legacyStartTime).TotalMilliseconds / (algorithmCompiledEndTime - algorithmCompiledStartTime).TotalMilliseconds) * 100 + "% "
                );
            }
        }
    }
}
