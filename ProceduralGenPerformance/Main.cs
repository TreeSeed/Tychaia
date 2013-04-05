//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

//#define MODULO_SPEED_TEST
#define VERIFICATION
//#define RANGED_LOGIC_TEST

using System;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Compiler;

namespace ProceduralGenPerformance
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Layer legacy = null;
            RuntimeLayer algorithmRuntime = null;
            IGenerator algorithmCompiled = null;
            IGenerator algorithmCompiledBuiltin = null;
            string mode = "quadzoom";
            if (args.Length > 0)
                mode = args[0];

            // Create initial layers from both types.
            if (mode == "quadzoom")
            {
                legacy = new LayerZoom(new LayerZoom(new LayerZoom(new LayerZoom(new LayerInitialLand()))));
                var algorithmZoom1 = new RuntimeLayer(new AlgorithmZoom2D());
                var algorithmZoom2 = new RuntimeLayer(new AlgorithmZoom2D());
                var algorithmZoom3 = new RuntimeLayer(new AlgorithmZoom2D());
                var algorithmZoom4 = new RuntimeLayer(new AlgorithmZoom2D());
                var algorithmInitialLand = new RuntimeLayer(new AlgorithmInitialBool());
                algorithmZoom4.SetInput(0, algorithmInitialLand);
                algorithmZoom3.SetInput(0, algorithmZoom4);
                algorithmZoom2.SetInput(0, algorithmZoom3);
                algorithmZoom1.SetInput(0, algorithmZoom2);
                algorithmRuntime = algorithmZoom1;
#if !RANGED_LOGIC_TEST
                algorithmCompiled = LayerCompiler.Compile(algorithmRuntime);
#endif
                //algorithmCompiledBuiltin = new CompiledLayerBuiltin();
            }
            else if (mode == "doublezoom")
            {
                legacy = new LayerZoom(new LayerZoom(new LayerInitialLand()));
                var algorithmZoom1 = new RuntimeLayer(new AlgorithmZoom2D());
                var algorithmZoom2 = new RuntimeLayer(new AlgorithmZoom2D());
                var algorithmInitialLand = new RuntimeLayer(new AlgorithmInitialBool());
                algorithmZoom2.SetInput(0, algorithmInitialLand);
                algorithmZoom1.SetInput(0, algorithmZoom2);
                algorithmRuntime = algorithmZoom1;
#if !RANGED_LOGIC_TEST
                algorithmCompiled = LayerCompiler.Compile(algorithmRuntime);
#endif
            }
            else if (mode == "zoom")
            {
                legacy = new LayerZoom(new LayerInitialLand());
                algorithmRuntime = new RuntimeLayer(new AlgorithmZoom2D());
                algorithmRuntime.SetInput(0, new RuntimeLayer(new AlgorithmInitialBool()));
#if !RANGED_LOGIC_TEST
                algorithmCompiled = LayerCompiler.Compile(algorithmRuntime);
#endif
            }
            else if (mode == "test")
            {
                var algorithmTest1 = new RuntimeLayer(new AlgorithmPassthrough());
                var algorithmTest2 = new RuntimeLayer(new AlgorithmPassthrough());
                var algorithmConstant = new RuntimeLayer(new AlgorithmConstant { Constant = 5 });
                algorithmTest2.SetInput(0, algorithmConstant);
                algorithmTest1.SetInput(0, algorithmTest2);
                algorithmRuntime = algorithmTest1;
#if !RANGED_LOGIC_TEST
                algorithmCompiled = LayerCompiler.Compile(algorithmRuntime);
#endif
            }
            else if (mode == "extend")
            {
                legacy = null;
                algorithmRuntime = new RuntimeLayer(new AlgorithmExtend());
                algorithmRuntime.SetInput(0, new RuntimeLayer(new AlgorithmInitialBool()));
#if !RANGED_LOGIC_TEST
                algorithmCompiled = LayerCompiler.Compile(algorithmRuntime);
#endif
            }
            else if (mode == "land")
            {
                legacy = new LayerInitialLand();
                algorithmRuntime = new RuntimeLayer(new AlgorithmInitialBool());
#if !RANGED_LOGIC_TEST
                algorithmCompiled = LayerCompiler.Compile(algorithmRuntime);
#endif
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
            try
            {
                int vcomputations;
                int[] legacyData = legacy.GenerateData(0, 0, 16, 16);
                int[] runtimeData = algorithmRuntime.GenerateData(0, 0, 0, 16, 16, 1, out vcomputations);
                int[] compiledData = algorithmCompiled.GenerateData(0, 0, 0, 16, 16, 1, out vcomputations);
                if (runtimeData.Length != legacyData.Length)
                {
                    Console.WriteLine("STOP: Runtime data evaluation results in a different array size than the legacy system.");
                    return;
                }
                if (compiledData.Length != legacyData.Length)
                {
                    Console.WriteLine("STOP: Compiled data evaluation results in a different array size than the legacy system.");
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
                        Console.WriteLine("WARNING: Compiled algorithm results in different data to legacy system.");
                        break;
                    }
                }
                for (var i = 0; i < legacyData.Length; i++)
                {
                    if (runtimeData[i] != compiledData[i])
                    {
                        Console.WriteLine("STOP: Runtime algorithm results in different data to compiled algorithm.");
                        for (var x = 0; x < 16; x++)
                        {
                            for (var y = 0; y < 16; y++)
                            {
                                if (runtimeData[x + y * 16] != compiledData[x + y * 16])
                                    Console.Write(compiledData[x + y * 16]);
                                else
                                    Console.Write(" ");
                            }
                            Console.WriteLine();
                        }
                        break;
                    }
                }
                Console.WriteLine("=== SAMPLE FROM RUNTIME =====");
                for (var x = 0; x < 16; x++)
                {
                    for (var y = 0; y < 16; y++)
                    {
                        Console.Write(runtimeData[x + y * 16]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("=== SAMPLE FROM COMPILED ====");
                for (var x = 0; x < 16; x++)
                {
                    for (var y = 0; y < 16; y++)
                    {
                        Console.Write(compiledData[x + y * 16]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("=============================");
            }
            catch
            {
                // The generation of data failed... could be a dodgy
                // algorithm implementation breaking.
                Console.WriteLine("Verification not possible (check algorithm implementations).");
            }

            #endregion

#endif

#if RANGED_LOGIC_TEST

            var ranged = new RangedLayer(algorithmRuntime);
            Console.WriteLine(ranged.GetPrintableStructure());
            
            ICSharpCode.NRefactory.CSharp.Expression ix, iy, iz, iwidth, iheight, idepth;
            RangedLayer.FindMaximumBounds(ranged, out ix, out iy, out iz, out iwidth, out iheight, out idepth);
            Console.WriteLine("The X expression is: " + ix.ToString());
            Console.WriteLine("The Y expression is: " + iy.ToString());
            Console.WriteLine("The Z expression is: " + iz.ToString());
            Console.WriteLine("The W expression is: " + iwidth.ToString());
            Console.WriteLine("The H expression is: " + iheight.ToString());
            Console.WriteLine("The D expression is: " + idepth.ToString());
            return;

#endif

            // Run tests to see how fast they are.
            for (int x = 0; x < 300; x++)
            {
                int computations = 0;
                var legacyFailed = false;
                var legacyStartTime = DateTime.Now;
                try
                {
                    if (legacy != null)
                    {
                        Console.WriteLine("Starting Test #" + x + " (legacy)");
                        for (int i = 0; i < 1000; i++)
                            legacy.GenerateData(0, 0, 128, 128);
                    }
                }
                catch
                {
                    legacyFailed = true;
                }
                var legacyEndTime = DateTime.Now;
                Console.WriteLine("Starting Test #" + x + " (algorithm runtime)");
                var algorithmRuntimeFailed = false;
                var algorithmRuntimeStartTime = DateTime.Now;
                try
                {
                    for (int i = 0; i < 1000; i++)
                        algorithmRuntime.GenerateData(0, 0, 0, 128, 128, 1, out computations);
                }
                catch
                {
                    algorithmRuntimeFailed = true;
                }
                var algorithmRuntimeEndTime = DateTime.Now;
                Console.WriteLine("Starting Test #" + x + " (algorithm compiled)");
                var algorithmCompiledFailed = false;
                var algorithmCompiledStartTime = DateTime.Now;
                try
                {
                    for (int i = 0; i < 1000; i++)
                        algorithmCompiled.GenerateData(0, 0, 0, 128, 128, 1, out computations);
                }
                catch
                {
                    algorithmCompiledFailed = true;
                }
                var algorithmCompiledEndTime = DateTime.Now;
                var algorithmCompiledBuiltinStartTime = DateTime.Now;
                var algorithmCompiledBuiltinEndTime = DateTime.Now;
                if (algorithmCompiledBuiltin != null)
                {
                    Console.WriteLine("Starting Test #" + x + " (algorithm compiled builtin)");
                    algorithmCompiledBuiltinStartTime = DateTime.Now;
                    for (int i = 0; i < 1000; i++)
                        algorithmCompiled.GenerateData(0, 0, 0, 128, 128, 1, out computations);
                    algorithmCompiledBuiltinEndTime = DateTime.Now;
                }
                // Because there are 1000 tests, and 1000 microseconds in a millisecond..
                Console.WriteLine(
                    "Test #" + x + " "
                    + (legacyFailed ? "" : ("LEGACY: " + (legacyEndTime - legacyStartTime).TotalMilliseconds + "µs "))
                    + (algorithmRuntimeFailed ? "" : ("ALGORITHM RUNTIME: " + (algorithmRuntimeEndTime - algorithmRuntimeStartTime).TotalMilliseconds + "µs "))
                    + (algorithmCompiledFailed ? "" : ("ALGORITHM COMPILED: " + (algorithmCompiledEndTime - algorithmCompiledStartTime).TotalMilliseconds + "µs "))
                    + ((algorithmCompiledBuiltin != null) ? ("ALGORITHM COMPILED BUILTIN: " + (algorithmCompiledBuiltinEndTime - algorithmCompiledBuiltinStartTime).TotalMilliseconds + "µs ") : "")
                    + "LC%: " + ((legacyEndTime - legacyStartTime).TotalMilliseconds / (algorithmCompiledEndTime - algorithmCompiledStartTime).TotalMilliseconds) * 100 + "% "
                );
            }
        }
    }
}
