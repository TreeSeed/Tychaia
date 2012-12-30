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

        /// <summary>
        /// Processes a runtime layer, returning the processing context so that the resulting
        /// code replicates and inlines the functionality of the algorithm.
        /// </summary>
        private static ProcessedResult ProcessRuntimeLayer(RuntimeLayer layer, IAlgorithm parent = null,
                                                           string width = "width", string height = "height", string depth = "depth")
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
            
            // Determine offsets for this loop.
            var xStartOffset = parent == null ? 0 : -parent.RequiredXBorder;
            var xEndOffset = parent == null ? 0 : (parent.RequiredXBorder) * 2;
            var yStartOffset = parent == null ? 0 : -parent.RequiredYBorder;
            var yEndOffset = parent == null ? 0 : (parent.RequiredYBorder) * 2;
            var zStartOffset = parent == null ? 0 : -parent.RequiredZBorder;
            var zEndOffset = parent == null ? 0 : (parent.RequiredZBorder) * 2;
            var halfWidth = parent == null ? 1 : (parent.InputWidthAtHalfSize ? 2 : 1);
            var halfHeight = parent == null ? 1 : (parent.InputHeightAtHalfSize ? 2 : 1);
            var halfDepth = parent == null ? 1 : (parent.InputDepthAtHalfSize ? 2 : 1);
            string newWidth = width, newHeight = height, newDepth = depth;
            if (halfWidth != 1)
                newWidth = "(" + newWidth + " / " + halfWidth + ")";
            if (xEndOffset != 0)
                newWidth = "(" + newWidth + " + " + xEndOffset + ")";
            if (halfHeight != 1)
                newHeight = "(" + newHeight + " / " + halfHeight + ")";
            if (yEndOffset != 0)
                newHeight = "(" + newHeight + " + " + yEndOffset + ")";
            if (halfDepth != 1)
                newDepth = "(" + newDepth + " / " + halfDepth + ")";
            if (zEndOffset != 0)
                newDepth = "(" + newDepth + " + " + zEndOffset + ")";
            if (newWidth != width)
            {
                width = "width_" + GenerateRandomIdentifier();
                result.Declarations += "int " + width + " = " + newWidth + ";\n";
            }
            if (newHeight != height)
            {
                height = "height_" + GenerateRandomIdentifier();
                result.Declarations += "int " + height + " = " + newHeight + ";\n";
            }
            if (newDepth != depth)
            {
                depth = "depth_" + GenerateRandomIdentifier();
                result.Declarations += "int " + depth + " = " + newDepth + ";\n";
            }

            // If the runtime layer has inputs, we need to process them first.
            if (layer.Algorithm.InputTypes.Length > 0)
            {
                var inputs = layer.GetInputs();
                result.InputVariableNames = new string[inputs.Length];
                for (var i = 0; i < inputs.Length; i++)
                {
                    var inputResult = ProcessRuntimeLayer(inputs[i], layer.Algorithm,
                                                          width, height, depth);
                    result.ProcessedCode += inputResult.ProcessedCode;
                    result.InputVariableNames[i] = inputResult.OutputVariableName;
                    result.Declarations += inputResult.Declarations;
                }
            }

            // Create an output variable definition.
            result.OutputVariableName = GenerateRandomIdentifier();
            result.OutputVariableType = algorithm.OutputType.FullName;
            result.Declarations += result.OutputVariableType + "[] " + result.OutputVariableName +
                " = new " + result.OutputVariableType + "[" + width + " * " + height + " * " + depth + "];\n";

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
                                             -xStartOffset, -yStartOffset, -zStartOffset,
                                             width, height, depth);
            AlgorithmRefactorer.RemoveUsingStatements(astBuilder.CompilationUnit, result.UsingStatements);
            var code = method.Body.GetText();

            // Surround the code with for loops.
            var forLoop = @"for (var k = 0; k < " + depth + @"; k++)
for (var i = 0; i < " + width + @"; i++)
for (var j = 0; j < " + height + @"; j++)
";
            code = forLoop + code;

            // Add the code to our processing result.
            result.ProcessedCode += code;

            // Return the processed result.
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

