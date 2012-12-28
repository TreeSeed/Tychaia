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

        private class ProcessingResult
        {
            public Dictionary<string, string> GlobalArrays = new Dictionary<string, string>();
            public List<string> UsingStatements = new List<string>();
            public string OutputVariableType = null;
            public string OutputVariableName = null;
            public string ProcessingCode = null;
            public IAlgorithm AlgorithmInstance = null;
        }

        /// <summary>
        /// This method accepts code decompiled by ICSharpCode and fixes it to work inline.
        /// </summary>
        private static void CleanupCode(ProcessingResult context, string code)
        {
            var result = "";
            var firstLine = true;

            // Add scope brackets.
            result += "// Starting scope for ProcessCall.\n";
            result += "{\n";

            // Process the code line-by-line.
            using (var memory = new MemoryStream(Encoding.ASCII.GetBytes(code)))
            {
                using (var reader = new StreamReader(memory))
                {
                    string processCallName;
                    List<KeyValuePair<string, string>> processCallParameters;

                    while (!reader.EndOfStream)
                    {
                        var lineUntrimmed = reader.ReadLine();
                        var line = lineUntrimmed.Trim();

                        // Skip blank lines.
                        if (line == "")
                            continue;

                        // Steal using statements.
                        if (line.StartsWith("using "))
                        {
                            context.UsingStatements.Add(line.Substring("using ".Length).TrimEnd(';'));
                            continue;
                        }

                        // If this is the first line, it should be a method declaration.
                        if (firstLine)
                        {
                            firstLine = false;

                            // Check and make sure the declaration is valid.
                            if (!line.StartsWith("public override void "))
                                throw new InvalidOperationException("Compiled ProcessCell is not declared 'public override void' after decompilation.");

                            // Reparse the process call definition.
                            ReparseProcessCallDefinition(line, out processCallName, out processCallParameters);

                            // Add the code at the start of this block to declare parameters as variables.
                            int i = 0;
                            foreach (var kv in processCallParameters)
                            {
                                if (i == 0)
                                    result += "var " + kv.Value + " = this;\n";
                                else if (i == 1)
                                    result += "var " + kv.Value + " = " + context.OutputVariableName + ";\n";
                                else if (i >= 2 && i < processCallParameters.Count - 6)
                                    result += "var " + kv.Value + " = /* FIXME */ null;\n";
                                else if (i == processCallParameters.Count - 6)
                                    result += "var " + kv.Value + " = __compiled_i;\n";
                                else if (i == processCallParameters.Count - 5)
                                    result += "var " + kv.Value + " = __compiled_j;\n";
                                else if (i == processCallParameters.Count - 4)
                                    result += "var " + kv.Value + " = __compiled_k;\n";
                                else if (i == processCallParameters.Count - 3)
                                    result += "var " + kv.Value + " = __compiled_width;\n";
                                else if (i == processCallParameters.Count - 2)
                                    result += "var " + kv.Value + " = __compiled_height;\n";
                                else if (i == processCallParameters.Count - 1)
                                    result += "var " + kv.Value + " = __compiled_depth;\n";
                                else
                                    throw new InvalidOperationException("Unknown parameter to rewrite as local variable.");
                                i++;
                            }
                        }
                        else
                        {
                            // Replace any algorithm properties with the actual values.
                            var propertiesToReplace = context.AlgorithmInstance.GetType().GetProperties();
                            foreach (var p in propertiesToReplace)
                            {
                                if (!lineUntrimmed.Contains("this." + p.Name) && 
                                    !lineUntrimmed.Contains(p.Name))
                                    continue;
                                var replacementValue = "";
                                if (p.PropertyType == typeof(int) || p.PropertyType == typeof(uint) ||
                                    p.PropertyType == typeof(long) || p.PropertyType == typeof(ulong) ||
                                    p.PropertyType == typeof(float) || p.PropertyType == typeof(double) ||
                                    p.PropertyType == typeof(bool))
                                    replacementValue = p.GetGetMethod().Invoke(context.AlgorithmInstance, null).ToString().ToLower();
                                else if (p.PropertyType == typeof(string))
                                    replacementValue = "\"" + p.GetGetMethod().Invoke(context.AlgorithmInstance, null).ToString() + "\"";
                                else
                                    throw new NotSupportedException("Unable to compile type '" + p.PropertyType.Name +
                                        "' for property '" + p.Name +
                                        "' in runtime algorithm '" + context.AlgorithmInstance.GetType().Name + "'.");
                                lineUntrimmed = lineUntrimmed.Replace("this." + p.Name, replacementValue);
                                lineUntrimmed = lineUntrimmed.Replace(p.Name, replacementValue);
                            }

                            // Replace methods that aren't named correctly.
                            lineUntrimmed = lineUntrimmed.Replace("context.GetRandom0", "context.GetRandomDouble");

                            // Omit checked / unchecked keywords.
                            lineUntrimmed = lineUntrimmed.Replace("unchecked", "");
                            lineUntrimmed = lineUntrimmed.Replace("checked", "");

                            // Change type names.
                            lineUntrimmed = lineUntrimmed.Replace("System.Int32", "int");
                            lineUntrimmed = lineUntrimmed.Replace("System.Int64", "long");

                            // Remove stupid casts.
                            //lineUntrimmed = lineUntrimmed.Replace("(int)(IntPtr)", "");

                            // Add the line onto the result.
                            result += lineUntrimmed + "\n";
                        }
                    }
                }
            }

            // Finish scope and return result code.
            result += "}\n";
            context.ProcessingCode = result;
        }

        /// <summary>
        /// Reparses the process call definition.
        /// </summary>
        private static void ReparseProcessCallDefinition(string definition, out string name, out List<KeyValuePair<string, string>> parameters)
        {
            // Retrieve the name.
            name = "";
            var chars = new Queue<char>(definition.ToCharArray());
            while (chars.Peek() != '(')
                name += chars.Dequeue();
            chars.Dequeue(); // Dequeue '('
            
            // Retrieve the parameters.
            parameters = new List<KeyValuePair<string, string>>();
            var isLexingType = true;
            var buffer = "";
            var bufferType = "";
            while (chars.Peek() != ')')
            {
                if (isLexingType)
                {
                    if (chars.Peek() == '<')
                        throw new NotSupportedException("Unable to reparse ProcessCell declarations containing generics.");
                    buffer += chars.Dequeue();
                    var gotSpace = false;
                    var gotBrackets = false;
                    while (chars.Peek() == ' ')
                    {
                        chars.Dequeue();
                        gotSpace = true;
                    }
                    if (chars.Peek() == '[')
                    {
                        while (chars.Peek() != ']')
                            chars.Dequeue();
                        chars.Dequeue();
                        buffer += "[]";
                        gotBrackets = true;
                    }
                    if (gotSpace || gotBrackets)
                    {
                        isLexingType = false;
                        bufferType = buffer;
                        buffer = "";
                    }
                }
                else
                {
                    while (chars.Peek() != ',' && chars.Peek() != ')')
                        buffer += chars.Dequeue();
                    if (chars.Peek() == ',')
                        chars.Dequeue();
                    while (chars.Peek() == ' ')
                        chars.Dequeue();
                    parameters.Add(new KeyValuePair<string, string>(bufferType, buffer));
                    bufferType = "";
                    buffer = "";
                    isLexingType = true;
                }
            }
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
        private static ProcessingResult ProcessRuntimeLayer(RuntimeLayer layer)
        {
            // Get a reference to the algorithm that the runtime layer is using.
            var algorithm = layer.Algorithm;
            if (algorithm == null)
                throw new InvalidOperationException("Attempted to compile null runtime layer!");
            var algorithmType = algorithm.GetType();

            // Create an output variable definition.
            var processingResult = new ProcessingResult();
            processingResult.ProcessingCode = "";
            processingResult.OutputVariableName = GenerateRandomIdentifier();
            processingResult.OutputVariableType = algorithm.OutputType.FullName;
            processingResult.AlgorithmInstance = algorithm;

            // Load Tychaia.ProceduralGeneration into Mono.Cecil.
            var module = AssemblyDefinition.ReadAssembly("Tychaia.ProceduralGeneration.dll").MainModule;

            // Now we have a reference to the method we want to decompile.
            TypeDefinition cecilType;
            MethodDefinition processCell;
            FindProcessCell(module, algorithmType, out processCell, out cecilType);
            var decompilerSettings = new DecompilerSettings();
            var astBuilder = new AstBuilder(new DecompilerContext(module) { CurrentType = cecilType, Settings = decompilerSettings });
            astBuilder.AddMethod(processCell);
            using (var writer = new StringWriter())
            {
                astBuilder.GenerateCode(new PlainTextOutput(writer));
                var code = writer.ToString();

                // We need to make some modifications to the code before we return it.
                CleanupCode(processingResult, code);
                
                // Prefix the looping constructs.
                processingResult.ProcessingCode = @"for (var __compiled_i = __compiled_x; __compiled_i < __compiled_x + __compiled_width; __compiled_i++)
for (var __compiled_j = __compiled_y; __compiled_j < __compiled_y + __compiled_height; __compiled_j++)
for (var __compiled_k = __compiled_z; __compiled_k < __compiled_z + __compiled_depth; __compiled_k++)
{" + "\n" + processingResult.ProcessingCode;

                // Prefix the array declarations.
                processingResult.ProcessingCode = processingResult.OutputVariableType + "[] " + processingResult.OutputVariableName + " = new " +
                    processingResult.OutputVariableType + "[__compiled_width * __compiled_height * __compiled_depth];\n" + processingResult.ProcessingCode;

                // Postfix the final result.
                processingResult.ProcessingCode = processingResult.ProcessingCode + "\n}\nreturn " + processingResult.OutputVariableName + ";\n";

                // Return the processing result.
                return processingResult;
            }
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
        private static IGenerator GenerateType(ProcessingResult result)
        {
            // Create the code using the template file.
            string template = null;
            using (var reader = new StreamReader("Compiler/CompiledLayerTemplate.cs"))
                template = reader.ReadToEnd();
            var test = 
                @"
            IRuntimeContext context = this;

            int[] output = new int[__compiled_width * __compiled_height * __compiled_depth];

            // Loop.
            for (var i = __compiled_x; i < __compiled_x + __compiled_width; i++)
                for (var j = __compiled_y; j < __compiled_y + __compiled_height; j++)
                    for (var k = __compiled_z; k < __compiled_z + __compiled_depth; k++)
                    {
                        if (true && i == 0 && j == 0)
                            output[i + j * __compiled_width + k * __compiled_width * __compiled_height] = 1;
                        else if (context.GetRandomDouble(i, j, k, context.Modifier) > 0.9)
                            output[i + j * __compiled_width + k * __compiled_width * __compiled_height] = 1;
                        else
                            output[i + j * __compiled_width + k * __compiled_width * __compiled_height] = 0;
                    }

            return output;";
            var final = template.Replace("/****** %CODE% ******/", test)//result.ProcessingCode)
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
            if (results.Errors.HasErrors)
            {
                Console.WriteLine(final);
                Console.WriteLine();
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

