//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Tychaia.ProceduralGeneration;
using Tychaia.ProceduralGeneration.Compiler;
using ICSharpCode.NRefactory.CSharp;
using Tychaia.ProceduralGeneration.AstVisitors;

namespace ProceduralGenCompiler
{
    class MainClass
    {
        private static void PrintRangedTree(RangedLayer layer, string indent = " ", string desired = "\x250E")
        {
            var i = "  ";
            var d = "\x250E";
            foreach (var input in layer.Inputs)
            {
                PrintRangedTree(input, indent + i, d);
                i = " \x2503";
                d = "\x2520";
            }
            var x = "\x2500\x2500";
            if (layer.Inputs.Length > 0)
                x = "\x2500\x2538";
            var line =
                indent.Substring(0, indent.Length - 1) +
                desired + x + " " +
                layer.Layer.Algorithm.GetType().FullName;
            while (line.Length < 80) line += " ";
            line += "runs from " + layer.X + ", " + layer.Y + ", " + layer.Z;
            Console.WriteLine(line);
            var writeLine = new Action<string>(s =>
            {
                var line2 = indent.Substring(0, indent.Length - 1) + "\x2503" + " " + " ";
                while (line2.Length < 80) line2 += " ";
                line2 += s;
                Console.WriteLine(line2);
            });
            writeLine("runs to   " + layer.OuterX + ", " + layer.OuterY + ", " + layer.OuterZ);
            writeLine("with size " + layer.Width + ", " + layer.Height + ", " + layer.Depth);
            writeLine(
                "calculation begins when i, j, k is " +
                layer.CalculationStartI + ", " +
                layer.CalculationStartJ + ", " +
                layer.CalculationStartK);
            writeLine(
                "calculation ends when i, j, k is " +
                layer.CalculationEndI + ", " +
                layer.CalculationEndJ + ", " +
                layer.CalculationEndK);
            writeLine(
                "during calculation i, j, k is offset by " +
                layer.OffsetI + ", " +
                layer.OffsetJ + ", " +
                layer.OffsetK);
            writeLine(
                "during calculation x, y, z is offset by " +
                layer.OffsetX + ", " +
                layer.OffsetY + ", " +
                layer.OffsetZ);
        }

        public static void Main(string[] args)
        {
            var constant = new RuntimeLayer(new AlgorithmConstant { Constant = 123456 });
            var perlin = new RuntimeLayer(new AlgorithmPerlin());
            var add = new RuntimeLayer(new AlgorithmAdd());
            var perlin2 = new RuntimeLayer(new AlgorithmPerlin());
            var passthrough = new RuntimeLayer(new AlgorithmPassthrough { XBorder = 7, YBorder = 9, ZBorder = 11 });
            var heightC = new RuntimeLayer(new AlgorithmHeightChange());
            var zoom3D = new RuntimeLayer(new AlgorithmZoom3D());
            passthrough.SetInput(0, perlin2);
            //zoom3D.SetInput(0, perlin);
            add.SetInput(0, perlin/*zoom3D*/);
            add.SetInput(1, passthrough);
            heightC.SetInput(0, add);
            var runtime = heightC;

            // ------- RANGED --------
            Expression xl, yl, zl, width, height, depth, outerX, outerY, outerZ;
            var ranged = new RangedLayer(runtime);
            PrintRangedTree(ranged);
            RangedLayer.FindMaximumBounds(
                ranged,
                out xl, out yl, out zl,
                out width, out height, out depth,
                out outerX, out outerY, out outerZ);
            Console.WriteLine();
            Console.WriteLine("Information about this structure:");
            Console.WriteLine(
                " * The minimum X, Y, Z positions to be encountered are: " +
                xl + ", " +
                yl + ", " +
                zl + ".");
            Console.WriteLine(
                " * The maximum X, Y, Z positions to be encountered are: " +
                outerX + ", " +
                outerY + ", " +
                outerZ + ".");
            Console.WriteLine();
            Console.WriteLine("How the processing will occur:");
            Console.WriteLine(
                " * The size of the storage arrays in the generated code will be: " +
                width + ", " +
                height + ", " +
                depth + ".");
            Console.WriteLine(" * The i, j, k values will start at 0, 0, 0.");
            Console.WriteLine(
                " * The i, j, k values will end at " +
                width + ", " +
                height + ", " +
                depth + ".");
            Console.WriteLine();

            // ------- TEST CODE -------

            var compiledCode = LayerCompiler.GenerateCode(runtime, false);

            Console.WriteLine("The generated code is:");
            Console.WriteLine();
            Console.WriteLine(compiledCode);

            var compiled = LayerCompiler.Compile(runtime);

            int computations;
            var runtimeData = runtime.GenerateData(-10, -10, -10, 20, 20, 20, out computations);
            var compiledData = compiled.GenerateData(-10, -10, -10, 20, 20, 20, out computations);
            var matches = true;
            var count = 0;
            var total = 0;
            for (var x = 0; x < 20; x++)
            for (var y = 0; y < 20; y++)
            for (var z = 0; z < 20; z++)
            {
                total += 1;
                if (runtimeData[x + y * 20 + z * 20 * 20] != compiledData[x + y * 20 + z * 20 * 20])
                {
                    count += 1;
                    /*Console.WriteLine("Runtime (" +
                    runtimeData[x + y * 20 + z * 20 * 20] +
                    ") at " + x + ", " + y + ", " + z + " doesn't match compiled (" +
                    compiledData[x + y * 20 + z * 20 * 20] + ").");*/
                    matches = false;
                }
            }
            if (matches)
                Console.WriteLine("Compiled layer matches runtime.");
            else
                Console.WriteLine("Compiled layer is " + (count / (double)total) * 100 + "% different to runtime.");
        }
    }
}

