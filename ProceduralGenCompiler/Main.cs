//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Compiler;

namespace ProceduralGenCompiler
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var initialBool = new RuntimeLayer(new AlgorithmInitialBool());
            var passthrough = new RuntimeLayer(new AlgorithmPassthrough());
            passthrough.SetInput(0, initialBool);

            var runtime = passthrough;
            var compiledCode = LayerCompiler.GenerateCode(runtime);
            var compiled = LayerCompiler.Compile(runtime);
            Console.WriteLine(compiledCode);

            int computations;
            var runtimeData = runtime.GenerateData(-10, -10, -10, 20, 20, 20, out computations);
            var compiledData = compiled.GenerateData(-10, -10, -10, 20, 20, 20, out computations);
            var matches = true;
            for (var x = 0; x < 20; x++)
            for (var y = 0; y < 20; y++)
            for (var z = 0; z < 20; z++)
            {
                if (runtimeData[x + y * 20 + z * 20 * 20] != compiledData[x + y * 20 + z * 20 * 20])
                {
                    Console.WriteLine("Runtime (" +
                    runtimeData[x + y * 20 + z * 20 * 20] +
                    ") at " + x + ", " + y + ", " + z + " doesn't match compiled (" +
                    compiledData[x + y * 20 + z * 20 * 20] + ").");
                    matches = false;
                }
            }
            if (matches)
                Console.WriteLine("Compiled layer matches runtime.");
        }
    }
}

