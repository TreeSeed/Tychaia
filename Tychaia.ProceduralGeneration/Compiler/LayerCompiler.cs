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
using Protogame;
using Tychaia.ProceduralGeneration.AstVisitors;
using Tychaia.Threading;

namespace Tychaia.ProceduralGeneration.Compiler
{
    public static class LayerCompiler
    {
        private static Random m_Random = new Random();

        public static IGenerator Compile(RuntimeLayer layer, bool optimize = true)
        {
            var result = ProcessRuntimeLayer(layer);
            return GenerateType(result, optimize);
        }

        public static string GenerateCode(RuntimeLayer layer, bool optimize = true)
        {
            var result = ProcessRuntimeLayer(layer);
            return GenerateCode(result, optimize);
        }

        internal static string GenerateRandomIdentifier()
        {
            var result = "";
            for (var i = 0; i < 8; i++)
                result += (char) ('a' + m_Random.Next(0, 26));
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

            // Add the conditional container.
            var code = "if (i >= " + ranged.CalculationStartI.GetText(null) + " && " +
                       "    j >= " + ranged.CalculationStartJ.GetText(null) + " && " +
                       "    k >= " + ranged.CalculationStartK.GetText(null) + " && " +
                       "    i <= " + ranged.CalculationEndI.GetText(null) + " && " +
                       "    j <= " + ranged.CalculationEndJ.GetText(null) + " && " +
                       "    k <= " + ranged.CalculationEndK.GetText(null) + ") {";

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
            AlgorithmRefactorer.InlineMethod(
                algorithm,
                method,
                result.OutputVariableName,
                result.InputVariableNames,
                new ParenthesizedExpression(
                    new BinaryOperatorExpression(
                        new BinaryOperatorExpression(
                            new IdentifierExpression("x"),
                            BinaryOperatorType.Add,
                            ranged.OffsetX.Clone()),
                        BinaryOperatorType.Add,
                        new BinaryOperatorExpression(
                            new IdentifierExpression("i"),
                            BinaryOperatorType.Add,
                            ranged.OffsetI.Clone()))),
                new ParenthesizedExpression(
                    new BinaryOperatorExpression(
                        new BinaryOperatorExpression(
                            new IdentifierExpression("y"),
                            BinaryOperatorType.Add,
                            ranged.OffsetY.Clone()),
                        BinaryOperatorType.Add,
                        new BinaryOperatorExpression(
                            new IdentifierExpression("j"),
                            BinaryOperatorType.Add,
                            ranged.OffsetJ.Clone()))),
                new ParenthesizedExpression(
                    new BinaryOperatorExpression(
                        new BinaryOperatorExpression(
                            new IdentifierExpression("z"),
                            BinaryOperatorType.Add,
                            ranged.OffsetZ.Clone()),
                        BinaryOperatorType.Add,
                        new BinaryOperatorExpression(
                            new IdentifierExpression("k"),
                            BinaryOperatorType.Add,
                            ranged.OffsetK.Clone()))),
                new ParenthesizedExpression(
                    new BinaryOperatorExpression(
                        new IdentifierExpression("i"),
                        BinaryOperatorType.Add,
                        ranged.OffsetI.Clone())),
                new ParenthesizedExpression(
                    new BinaryOperatorExpression(
                        new IdentifierExpression("j"),
                        BinaryOperatorType.Add,
                        ranged.OffsetJ.Clone())),
                new ParenthesizedExpression(
                    new BinaryOperatorExpression(
                        new IdentifierExpression("k"),
                        BinaryOperatorType.Add,
                        ranged.OffsetK.Clone())),
                new IdentifierExpression("__cwidth"),
                new IdentifierExpression("__cheight"),
                new IdentifierExpression("__cdepth"),
                new PrimitiveExpression(0),
                new PrimitiveExpression(0),
                new PrimitiveExpression(0));
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
            RangedLayer.FindMaximumBounds(
                ranged,
                out ix, out iy, out iz,
                out iwidth, out iheight, out idepth,
                out iouterx, out ioutery, out iouterz);

            // Add __cwidth, __cheight and __cdepth declarations.
            result.Declarations += "int __cx = (int)(" + ix.GetText(null) + ");\n";
            result.Declarations += "int __cy = (int)(" + iy.GetText(null) + ");\n";
            result.Declarations += "int __cz = (int)(" + iz.GetText(null) + ");\n";
            result.Declarations += "int __cwidth = " + iwidth.GetText(null) + ";\n";
            result.Declarations += "int __cheight = " + iheight.GetText(null) + ";\n";
            result.Declarations += "int __cdepth = " + idepth.GetText(null) + ";\n";

            // Create the for loop that our calculations are done within.
            result.ProcessedCode +=
                "for (var k = 0; k < " + idepth.GetText(null) + @"; k++)" +
                "for (var i = 0; i < " + iwidth.GetText(null) + @"; i++)" +
                "for (var j = 0; j < " + iheight.GetText(null) + @"; j++)" +
                "{";

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

        private static string GenerateCode(ProcessedResult result, bool optimize)
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
                .Replace("/****** %OUTPUT_VAR% ******/", result.OutputVariableName)
                .Replace("/****** %OUTPUT_TYPE% ******/", result.OutputVariableType)
                .Replace("/****** %DECLS% ******/", result.Declarations)
                .Replace("/****** %USING% ******/",
                    result.UsingStatements
                        .Where(v => v != "System" && v != "Tychaia.ProceduralGeneration")
                        .Select(v => "using " + v + ";")
                        .DefaultIfEmpty("")
                        .Aggregate((a, b) => a + "\n" + b));
            var parser = new CSharpParser();
            var tree = parser.Parse(final, "layer.cs");
            if (optimize)
            {
                AstHelpers.OptimizeCompilationUnit(tree);
            }
            var stringWriter = new StringWriter();
            var formatter = FormattingOptionsFactory.CreateMono();
            formatter.SpaceBeforeMethodCallParentheses = false;
            formatter.SpaceBeforeIndexerDeclarationBracket = false;
            tree.AcceptVisitor(new CSharpOutputVisitor(new TextWriterOutputFormatter(stringWriter)
            {
                IndentationString = "  "
            }, formatter));
            final = stringWriter.ToString();
            return final;
        }

        /// <summary>
        /// Generates the compiled type.
        /// </summary>
        private static IGenerator GenerateType(ProcessedResult result, bool optimize = true)
        {
            var final = GenerateCode(result, optimize);

            // Create the type.
            var parameters = new CompilerParameters(new[]
            {
                Assembly.GetExecutingAssembly().Location,
                typeof(PerlinNoise).Assembly.Location,
                typeof(InlineTaskPipeline<>).Assembly.Location,
                "System.Core.dll"
            });
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.IncludeDebugInformation = false;
            parameters.CompilerOptions = "/optimize";
            var compiler = CodeDomProvider.CreateProvider("CSharp");
            var results = compiler.CompileAssemblyFromSource(parameters, final);
            if (results.Errors.HasErrors)
            {
                foreach (var error in results.Errors)
                    Console.WriteLine(error);
                Console.WriteLine();
                throw new InvalidOperationException(
                    "Unable to compile code for layer generation.  Compiled code contained errors.");
            }
            var assembly = results.CompiledAssembly;
            var newType = assembly.GetType("CompiledLayer");
            return newType.GetConstructor(Type.EmptyTypes).Invoke(null) as IGenerator;
        }

        private class ProcessedResult
        {
            public IAlgorithm AlgorithmInstance = null;
            public string Declarations = null;
            public string InitializationCode = null;
            public string[] InputVariableNames = null;
            public string OutputVariableName = null;
            public string OutputVariableType = null;
            public string ProcessedCode = null;

            public List<string> UsingStatements = new List<string>();
        }
    }
}