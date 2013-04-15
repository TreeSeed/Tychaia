//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using Mono.Cecil;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using System.Linq;

namespace Tychaia.ProceduralGeneration.Compiler
{
    public static class DecompileUtil
    {
        /// <summary>
        /// Returns the code for a specific algorithm.
        /// </summary>
        /// <returns>The algorithm code.</returns>
        /// <param name="algorithmType">Algorithm type.</param>
        public static MethodDeclaration GetAlgorithmCode(Type algorithmType, out AstBuilder astBuilder)
        {
            // Load Tychaia.ProceduralGeneration into Mono.Cecil.
            var module = AssemblyDefinition.ReadAssembly("Tychaia.ProceduralGeneration.dll").MainModule;
            
            // Now we have a reference to the method we want to decompile.
            TypeDefinition cecilType;
            MethodDefinition processCell;
            DecompileUtil.FindProcessCell(module, algorithmType, out processCell, out cecilType);
            var decompilerSettings = new DecompilerSettings();
            astBuilder = new AstBuilder(new DecompilerContext(module) { CurrentType = cecilType, Settings = decompilerSettings });
            astBuilder.AddMethod(processCell);
            astBuilder.RunTransformations();
            astBuilder.CompilationUnit.AcceptVisitor(new InsertParenthesesVisitor {
                InsertParenthesesForReadability = true
            });
            
            // Return.
            return astBuilder.CompilationUnit.Members.Where(v => v is MethodDeclaration).Cast<MethodDeclaration>().First();
        }

        /// <summary>
        /// Finds the Mono.Cecil.MethodDefinition for ProcessCell in the specified algorithm type.
        /// </summary>
        public static void FindProcessCell(ModuleDefinition module,
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
    }
}

