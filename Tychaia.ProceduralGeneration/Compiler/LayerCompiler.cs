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

        //
        //
        //     need to have one function to calculate maximum loop required from "store result" -> first layer
        //
        //     need to have another function (or passing parameters into each CompileLayer call) for calculating
        //     x, y, z, width, height, depth of current layer if statement.
        //
        //     could pass in x, y, z, width, height, depth into each CompileLayer with the starting values for
        //     "store result" set to the offset x, y, z in the array (so that as it goes up layers, it subtracts
        //     from that position just as offsets do logically) and width, height, depth is divided by two or 
        //     subtracted by border * 2 where needed.
        //
        //     also can grab the same width / height / depth logic from the non-single loop version and just keep
        //     AddBorderEdgesRecursive around for figuring out the initial array maximum size
        //
        //

#if FALSE

        /// <summary>
        /// Works out the main loop bounds that we need to use in order to calculate
        /// the entire layer system.
        /// </summary>
        private static void AddBorderEdgesRecursive(RuntimeLayer layer, IAlgorithm current,
                                                    ref string absoluteX, ref string absoluteY, ref string absoluteZ,
                                                    ref string absoluteWidth, ref string absoluteHeight, ref string absoluteDepth)
        {
            // Handle parent layers.
            if (layer.GetInputs().Length > 0)
            {
                var l = layer.GetInputs()[0];
                AddBorderEdgesRecursive(l, layer.Algorithm,
                                        ref absoluteX, ref absoluteY, ref absoluteZ,
                                        ref absoluteWidth, ref absoluteHeight, ref absoluteDepth);
            }
            // Handle current.
            if (current != null)
            {
                if (current.RequiredXBorder != 0) 
                    absoluteX = "(" + absoluteX + " + " + current.RequiredXBorder + ")";
                if (current.RequiredYBorder != 0)
                    absoluteY = "(" + absoluteY + " + " + current.RequiredYBorder + ")";
                if (current.RequiredZBorder != 0)
                    absoluteZ = "(" + absoluteZ + " + " + current.RequiredZBorder + ")";
                if (!current.InputWidthAtHalfSize)
                {
                    if (current.RequiredXBorder != 0)
                        absoluteWidth = "(" + absoluteWidth + " + " + current.RequiredXBorder * 2 + ")";
                }
                else
                    absoluteWidth = "(" + absoluteWidth + " / 2)";
                if (!current.InputHeightAtHalfSize)
                {
                    if (current.RequiredYBorder != 0)
                        absoluteHeight = "(" + absoluteHeight + " + " + current.RequiredYBorder * 2 + ")";
                }
                else
                    absoluteHeight = "(" + absoluteHeight + " / 2)";
                if (!current.InputDepthAtHalfSize)
                {
                    if (current.RequiredZBorder != 0)
                        absoluteDepth = "(" + absoluteDepth + " + " + current.RequiredZBorder * 2 + ")";
                }
                else
                    absoluteDepth = "(" + absoluteDepth + " / 2)";
            }

            /* FIXME: Work for multiple layers.
            int temporaryX = 0, temporaryY = 0, temporaryZ = 0, temporaryWidth = 0, temporaryHeight = 0, temporaryDepth = 0;
            int maximumX = 0, maximumY = 0, maximumZ = 0, maximumWidth = 0, maximumHeight = 0, maximumDepth = 0;
            foreach (var l in layer.GetInputs())
            {
                AddBorderEdgesRecursive(l, layer.Algorithm,
                                        ref temporaryX, ref temporaryY, ref temporaryZ,
                                        ref temporaryWidth, ref temporaryHeight, ref temporaryDepth);
                if (temporaryX < maximumX)
                    maximumX = temporaryX;
                if (temporaryY < maximumY)
                    maximumY = temporaryY;
                if (temporaryZ < maximumZ)
                    maximumZ = temporaryZ;
                if (temporaryWidth > maximumWidth)
                    maximumWidth = temporaryWidth;
                if (temporaryHeight > maximumHeight)
                    maximumHeight = temporaryHeight;
                if (temporaryDepth > maximumDepth)
                    maximumDepth = temporaryDepth;
            }
            absoluteX += maximumX;
            absoluteY += maximumY;
            absoluteZ += maximumZ;
            absoluteWidth += maximumWidth;
            absoluteHeight += maximumHeight;
            absoluteDepth += maximumDepth;
            */
        }

#endif

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

            // Work out bounds.
            ICSharpCode.NRefactory.CSharp.Expression ix, iy, iz, iwidth, iheight, idepth;
            RangedLayer.FindMaximumBounds(ranged, out ix, out iy, out iz, out iwidth, out iheight, out idepth);

            // Add the conditional container.
            string code = "if (k >= (int)((" + iz + ") - z) && i >= (int)((" + ix + ") - x) && j >= (int)((" + iy + ") - y)" + 
                " && k < " + idepth + " && i < " + iwidth + " && j < " + iheight + @")
{
";

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
            ICSharpCode.NRefactory.CSharp.Expression ix, iy, iz, iwidth, iheight, idepth;
            RangedLayer.FindMaximumBounds(ranged, out ix, out iy, out iz, out iwidth, out iheight, out idepth);

            // Add __cwidth, __cheight and __cdepth declarations.
            result.Declarations += "int __cx = (int)(x - (" + ix + "));\n";
            result.Declarations += "int __cy = (int)(y - (" + iy + "));\n";
            result.Declarations += "int __cz = (int)(z - (" + iz + "));\n";
            result.Declarations += "int __cwidth = (int)(x - (" + ix + ")) + " + iwidth + ";\n";
            result.Declarations += "int __cheight = (int)(y - (" + iy + ")) + " + iheight + ";\n";
            result.Declarations += "int __cdepth = (int)(z - (" + iz + ")) + " + idepth + ";\n";

            // Create the for loop that our calculations are done within.
            result.ProcessedCode += @"for (var k = (int)((" + iz + ") - z); k < " + idepth + @"; k++)
for (var i = (int)((" + ix + ") - x); i < " + iwidth + @"; i++)
for (var j = (int)((" + iy + ") - y); j < " + iheight + @"; j++)
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

