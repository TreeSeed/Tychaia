//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using Tychaia.ProceduralGeneration.AstVisitors;

namespace Tychaia.ProceduralGeneration.Compiler
{
    public static class LayerCompiler
    {
        public static IGenerator Compile(RuntimeLayer layer)
        {
            var result = ProcessRuntimeLayer(layer);
            return GenerateType(result);
        }

        public static string GenerateCode(RuntimeLayer layer)
        {
            var result = ProcessRuntimeLayer(layer);
            return GenerateCode(result);
        }

        private class ProcessedResult
        {
            public string ProcessedCode = null;
            public string InitializationCode = null;
            public string OutputVariableType = null;
            public string OutputVariableName = null;
            public string[] InputVariableNames = null;
            public string Declarations = null;

            public List<string> UsingStatements = new List<string>();
            public IAlgorithm AlgorithmInstance = null;
        }

        private static Random m_Random = new Random();
        internal static string GenerateRandomIdentifier()
        {
            var result = "";
            for (var i = 0; i < 8; i++)
                result += (char)((int)'a' + m_Random.Next(0, 26));
            return result;
        }

        private static ProcessedResult CompileRuntimeLayer(RuntimeLayer layer, RangedLayer ranged, IAlgorithm parent)
        {
            // Create our own processed result; a copy of our own state plus
            // somewhere to accumulate code.
            var result = new ProcessedResult();
            result.ProcessedCode = "";
            result.InitializationCode = "";

            // Get a reference to the algorithm that the runtime layer is using.
            var algorithm = layer.Algorithm;
            if (algorithm == null)
                throw new InvalidOperationException("Attempted to compile null runtime layer!");
            var algorithmType = algorithm.GetType();

            // If the runtime layer has inputs, we need to process them first.
            var inputs = layer.GetInputs();
            result.InputVariableNames = new string[inputs.Length];
            for (var i = 0; i < inputs.Length; i++)
            {
                var inputResult = CompileRuntimeLayer(inputs[i], ranged.Inputs[i], layer.Algorithm);
                result.ProcessedCode += inputResult.ProcessedCode;
                result.InitializationCode += inputResult.InitializationCode;
                result.InputVariableNames[i] = inputResult.OutputVariableName;
                result.Declarations += inputResult.Declarations;
                result.UsingStatements.AddRange(inputResult.UsingStatements);
            }

            // Create the storage array.
            result.OutputVariableName = GenerateRandomIdentifier();
            result.OutputVariableType = algorithm.OutputType.FullName;
            result.Declarations += result.OutputVariableType + "[] " + result.OutputVariableName +
                " = new " + result.OutputVariableType + "[__cwidth * __cheight * __cdepth];\n";

            Console.WriteLine(ranged.ToString());

            // Add the conditional container.
            string code = "if (k >= (int)((" + ranged.Z.GetText(null) + ") - z) && i >= (int)((" + ranged.X.GetText(null) + ") - x) && j >= (int)((" + ranged.Y.GetText(null) + ") - y)" +
                " && k < " + ranged.OuterZ.GetText(null) + " && i < " + ranged.OuterX.GetText(null) + " && j < " + ranged.OuterY.GetText(null) + @")
{
";

            /*result.Declarations += "Console.WriteLine(\"COMPILED: \" + " +
                "" + ranged.X + " + \" \" + " +
                "" + ranged.Y + " + \" \" + " +
                "" + ranged.Z + " + \" \" + " +
                "" + ranged.Width + " + \" \" + " +
                "" + ranged.Height + " + \" \" + " +
                "" + ranged.Depth + ");";*/

            // Refactor the method.
            AstBuilder astBuilder;
            var method = DecompileUtil.GetMethodCode(algorithmType, out astBuilder, "ProcessCell");
            MethodDeclaration initialize = null;
            try
            {
                initialize = DecompileUtil.GetMethodCode(algorithmType, out astBuilder, "Initialize");
            }
            catch (MissingMethodException)
            {
            }
            AlgorithmRefactorer.InlineMethod(algorithm, method, result.OutputVariableName, result.InputVariableNames,
                                             "__cx", "__cy", "__cz",
                                             "__cwidth", "__cheight", "__cdepth",
                                             0, 0, 0);
            if (initialize != null)
                AlgorithmRefactorer.InlineInitialize(algorithm, initialize);
            AlgorithmRefactorer.RemoveUsingStatements(astBuilder.CompilationUnit, result.UsingStatements);
            AlgorithmRefactorer.FactorOutAlgorithmFields(algorithmType, method, initialize, ref result.Declarations);
            code += method.Body.GetText();

            // Terminate the conditional container and return.
            code += "computations += 1;";
            code += "}\n";
            result.ProcessedCode += code;
            if (initialize != null)
                result.InitializationCode += initialize.Body.GetText();
            return result;
        }

        private static ProcessedResult ProcessRuntimeLayer(RuntimeLayer layer)
        {
            // Create our own processed result; a copy of our own state plus
            // somewhere to accumulate code.
            var result = new ProcessedResult();
            result.ProcessedCode = "";
            result.InitializationCode = "";

            // Get a reference to the algorithm that the runtime layer is using.
            var algorithm = layer.Algorithm;
            if (algorithm == null)
                throw new InvalidOperationException("Attempted to compile null runtime layer!");

            // Use RangedLayer to work out the metrics.
            var ranged = new RangedLayer(layer);
            Expression ix, iy, iz, iwidth, iheight, idepth, iouterx, ioutery, iouterz;
            RangedLayer.FindMaximumBounds(ranged, out ix, out iy, out iz, out iwidth, out iheight, out idepth, out iouterx, out ioutery, out iouterz);

            // Add __cwidth, __cheight and __cdepth declarations.
            result.Declarations += "int __cx = (int)((x - (" + ix.GetText(null) + ")));\n";
            result.Declarations += "int __cy = (int)((y - (" + iy.GetText(null) + ")));\n";
            result.Declarations += "int __cz = (int)((z - (" + iz.GetText(null) + ")));\n";
            result.Declarations += "int __cwidth = " + iwidth.GetText(null) + ";\n";
            result.Declarations += "int __cheight = " + iheight.GetText(null) + ";\n";
            result.Declarations += "int __cdepth = " + idepth.GetText(null) + ";\n";

            // Create the for loop that our calculations are done within.
            result.ProcessedCode += @"for (var k = (int)((" + iz.GetText(null) + ") - z); k < " + iouterz.GetText(null) + @"; k++)
for (var i = (int)((" + ix.GetText(null) + ") - x); i < " + iouterx.GetText(null) + @"; i++)
for (var j = (int)((" + iy.GetText(null) + ") - y); j < " + ioutery.GetText(null) + @"; j++)
{
";

            // Now add the code for the layer.
            var inputResult = CompileRuntimeLayer(layer, ranged, null);
            result.ProcessedCode += inputResult.ProcessedCode;
            result.InitializationCode += inputResult.InitializationCode;
            result.OutputVariableName = inputResult.OutputVariableName;
            result.OutputVariableType = inputResult.OutputVariableType;
            result.Declarations += inputResult.Declarations;
            result.UsingStatements.AddRange(inputResult.UsingStatements);

            // Terminate the for loop and return the result.
            result.ProcessedCode += "}";
            return result;
        }

        private static string GenerateCode(ProcessedResult result)
        {
            // Create the code using the template file.
            var templateName = "Tychaia.ProceduralGeneration.Compiler.CompiledLayerTemplate.cs";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(templateName);
            string template = null;
            using (var reader = new StreamReader(stream))
                template = reader.ReadToEnd();
            var final = template
                .Replace("/****** %CODE% ******/", result.ProcessedCode)
                .Replace("/****** %INIT% ******/", result.InitializationCode)
                .Replace("/****** %RETURN% ******/", "return " + result.OutputVariableName + ";")
                .Replace("/****** %DECLS% ******/", result.Declarations)
                .Replace("/****** %USING% ******/",
                     result.UsingStatements
                        .Where(v => v != "System" && v != "Tychaia.ProceduralGeneration")
                        .Select(v => "using " + v + ";")
                        .DefaultIfEmpty("")
                        .Aggregate((a, b) => a + "\n" + b));
            var parser = new CSharpParser();
            var tree = parser.Parse(final, "layer.cs");
            tree.AcceptVisitor(new SimplifyCombinedMathExpressionsVisitor());
            tree.AcceptVisitor(new SimplifyZeroAndConditionalExpressionsVisitor());
            tree.AcceptVisitor(new SimplifyRedundantMathExpressionsVisitor());
            tree.AcceptVisitor(new RemoveRedundantPrimitiveCastsVisitor());
            tree.AcceptVisitor(new RemoveParenthesisVisitor());
            var stringWriter = new StringWriter();
            var formatter = FormattingOptionsFactory.CreateMono();
            formatter.SpaceBeforeMethodCallParentheses = false;
            formatter.SpaceBeforeIndexerDeclarationBracket = false;
            tree.AcceptVisitor(new CSharpOutputVisitor(new TextWriterOutputFormatter(stringWriter) {
                IndentationString = "  "
            }, formatter));
            final = stringWriter.ToString();
            return final;
        }

        /// <summary>
        /// Generates the compiled type.
        /// </summary>
        private static IGenerator GenerateType(ProcessedResult result)
        {
            var final = GenerateCode(result);

            // Create the type.
            var parameters = new CompilerParameters(new string[]
            {
                Assembly.GetExecutingAssembly().Location,
                typeof(Protogame.Noise.PerlinNoise).Assembly.Location,
                "System.Core.dll"
            });
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.IncludeDebugInformation = false;
            parameters.CompilerOptions = "/optimize";
            var compiler = CodeDomProvider.CreateProvider("CSharp");
            var results = compiler.CompileAssemblyFromSource(parameters, final);
            using (var writer = new StreamWriter("code.tmp.cs"))
            {
                //int i = 1;
                foreach (var line in final.Split('\n'))
                    //i++.ToString().PadLeft(4) + ":  " +
                    writer.WriteLine(line);
            }
            if (results.Errors.HasErrors)
            {
                foreach (var error in results.Errors)
                    Console.WriteLine(error);
                Console.WriteLine();
                throw new InvalidOperationException("Unable to compile code for layer generation.  Compiled code contained errors.");
            }
            var assembly = results.CompiledAssembly;
            var newType = assembly.GetType("CompiledLayer");
            return newType.GetConstructor(Type.EmptyTypes).Invoke(null) as IGenerator;
        }
    }
}

