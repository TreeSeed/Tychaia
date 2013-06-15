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
            var perlin = new RuntimeLayer(new AlgorithmPerlin());
            var zoom3D1 = new RuntimeLayer(new AlgorithmZoom3D());
            var zoom3D2 = new RuntimeLayer(new AlgorithmZoom3D());
            var zoom3D3 = new RuntimeLayer(new AlgorithmZoom3D());
            zoom3D1.SetInput(0, perlin);
            zoom3D2.SetInput(0, zoom3D1);
            zoom3D3.SetInput(0, zoom3D2);

            //var runtime = perlin;

            var passthrough = new RuntimeLayer(new AlgorithmPassthrough { XBorder = 2 });
            passthrough.SetInput(0, perlin);
            var runtime = passthrough;


            // ------- TEST CODE -------


            var compiledCode = LayerCompiler.GenerateCode(runtime);
            Console.WriteLine(compiledCode);

            var compiled = LayerCompiler.Compile(runtime);

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

