//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using Mono.Cecil;
using System.Text;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;

namespace Tychaia.ProceduralGeneration.Compiler
{
    public static class LayerCompiler
    {
        public static IGenerator Compile(RuntimeLayer layer)
        {
            // TODO: Work out the tree of runtime layers to determine what to compile.
            // TODO: Determine the actual code to generate by using Mono.Cecil on the assemblies.
            // TODO: Generate the code and return the type for use.

            //Console.WriteLine("Tracing compilation...");

            var result = ProcessRuntimeLayer(layer);
            return GenerateType(result);
        }

        private class ProcessedResult
        {
            public string ProcessedCode = null;
            public string OutputVariableType = null;
            public string OutputVariableName = null;
            public string[] InputVariableNames = null;
            public string Declarations = null;

            public List<string> UsingStatements = new List<string>();
            public IAlgorithm AlgorithmInstance = null;
        }

        private static Random m_Random = new Random();
        private static string GenerateRandomIdentifier()
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

            // Get a reference to the algorithm that the runtime layer is using.
            var algorithm = layer.Algorithm;
            if (algorithm == null)
                throw new InvalidOperationException("Attempted to compile null runtime layer!");
            var algorithmType = algorithm.GetType();
            
            // If the runtime layer has inputs, we need to process them first.
            if (layer.Algorithm.InputTypes.Length > 0)
            {
                var inputs = layer.GetInputs();
                result.InputVariableNames = new string[inputs.Length];
                for (var i = 0; i < inputs.Length; i++)
                {
                    var inputResult = CompileRuntimeLayer(inputs[i], ranged.Inputs[i], layer.Algorithm);
                    result.ProcessedCode += inputResult.ProcessedCode;
                    result.InputVariableNames[i] = inputResult.OutputVariableName;
                    result.Declarations += inputResult.Declarations;
                }
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

            // Load Tychaia.ProceduralGeneration into Mono.Cecil.
            var module = AssemblyDefinition.ReadAssembly("Tychaia.ProceduralGeneration.dll").MainModule;

            // Now we have a reference to the method we want to decompile.
            TypeDefinition cecilType;
            MethodDefinition processCell;
            FindProcessCell(module, algorithmType, out processCell, out cecilType);
            var decompilerSettings = new DecompilerSettings();
            var astBuilder = new AstBuilder(new DecompilerContext(module) { CurrentType = cecilType, Settings = decompilerSettings });
            astBuilder.AddMethod(processCell);
            astBuilder.RunTransformations();
            astBuilder.CompilationUnit.AcceptVisitor(new InsertParenthesesVisitor {
                InsertParenthesesForReadability = true
            });
            
            // Refactor the method.
            var method = astBuilder.CompilationUnit.Members.Where(v => v is MethodDeclaration).Cast<MethodDeclaration>().First();
            AlgorithmRefactorer.InlineMethod(algorithm, method, result.OutputVariableName, result.InputVariableNames,
                                             "__cx", "__cy", "__cz",
                                             "__cwidth", "__cheight", "__cdepth");
            AlgorithmRefactorer.RemoveUsingStatements(astBuilder.CompilationUnit, result.UsingStatements);
            code += method.Body.GetText();

            // Terminate the conditional container and return.
            code += "computations += 1;";
            code += "}\n";
            result.ProcessedCode += code;
            return result;
        }

        private static ProcessedResult ProcessRuntimeLayer(RuntimeLayer layer)
        {
            // Create our own processed result; a copy of our own state plus
            // somewhere to accumulate code.
            var result = new ProcessedResult();
            result.ProcessedCode = "";
            
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
            result.OutputVariableName = inputResult.OutputVariableName;
            result.OutputVariableType = inputResult.OutputVariableType;
            result.Declarations += inputResult.Declarations;

            // Terminate the for loop and return the result.
            result.ProcessedCode += "}";
            return result;
        }

        /// <summary>
        /// Finds the Mono.Cecil.MethodDefinition for ProcessCell in the specified algorithm type.
        /// </summary>
        private static void FindProcessCell(ModuleDefinition module,
                                            Type algorithmType,
                                            out MethodDefinition methodDefinition,
                                            out TypeDefinition typeDefinition)
        {
            foreach (var t in module.Types)
            {
                if (t.FullName == algorithmType.FullName)
                {
                    foreach (var m in t.Methods)
                    {
                        if (m.Name == "ProcessCell")
                        {
                            methodDefinition = m;
                            typeDefinition = t;
                            return;
                        }
                    }
                }
            }
            throw new MissingMethodException();
        }

        /// <summary>
        /// Generates the compiled type.
        /// </summary>
        private static IGenerator GenerateType(ProcessedResult result)
        {
            // Create the code using the template file.
            string template = null;
            using (var reader = new StreamReader("Compiler/CompiledLayerTemplate.cs"))
                template = reader.ReadToEnd();
            var final = template
                .Replace("/****** %CODE% ******/", result.ProcessedCode)
                .Replace("/****** %RETURN% ******/", "return " + result.OutputVariableName + ";")
                .Replace("/****** %DECLS% ******/", result.Declarations)
                .Replace("/****** %USING% ******/",
                     result.UsingStatements
                        .Where(v => v != "System" && v != "Tychaia.ProceduralGeneration")
                        .Select(v => "using " + v + ";")
                        .DefaultIfEmpty("")
                        .Aggregate((a, b) => a + "\n" + b));

            // Create the type.
            var parameters = new CompilerParameters(new string[]
            {
                "Tychaia.ProceduralGeneration.dll",
                "System.Core.dll"
            });
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.IncludeDebugInformation = false;
            parameters.CompilerOptions = "/optimize";
            var compiler = CSharpCodeProvider.CreateProvider("CSharp");
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

