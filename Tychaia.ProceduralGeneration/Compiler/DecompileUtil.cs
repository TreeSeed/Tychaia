// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;

namespace Tychaia.ProceduralGeneration.Compiler
{
    public static class DecompileUtil
    {
        /// <summary>
        /// Returns the code for a specific algorithm.
        /// </summary>
        /// <returns>The algorithm code.</returns>
        /// <param name="algorithmType">Algorithm type.</param>
        public static MethodDeclaration GetMethodCode(Type algorithmType, out AstBuilder astBuilder, string methodName)
        {
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName);
            var parameters = new ReaderParameters
            {
                AssemblyResolver = resolver,
            };

            // Load Tychaia.ProceduralGeneration into Mono.Cecil.
            var module = AssemblyDefinition.ReadAssembly(
                Assembly.GetExecutingAssembly().Location,
                parameters).MainModule;

            // Now we have a reference to the method we want to decompile.
            TypeDefinition cecilType;
            MethodDefinition processCell;
            FindMethodName(module, algorithmType, methodName, out processCell, out cecilType);
            var decompilerSettings = new DecompilerSettings();
            astBuilder =
                new AstBuilder(new DecompilerContext(module) { CurrentType = cecilType, Settings = decompilerSettings });
            astBuilder.AddMethod(processCell);
            try
            {
                astBuilder.RunTransformations();
            }
            catch (AssemblyResolutionException ex)
            {
                throw new Exception(
                    "Unable to decompile algorithm source code for " + algorithmType.FullName + ".",
                    ex);
            }

            astBuilder.CompilationUnit.AcceptVisitor(new InsertParenthesesVisitor
            {
                InsertParenthesesForReadability = true
            });

            // Return.
            return
                astBuilder.CompilationUnit.Members.Where(v => v is MethodDeclaration).Cast<MethodDeclaration>().First();
        }

        /// <summary>
        /// Finds the Mono.Cecil.MethodDefinition for ProcessCell in the specified algorithm type.
        /// </summary>
        public static void FindMethodName(
            ModuleDefinition module,
            Type algorithmType,
            string methodName,
            out MethodDefinition methodDefinition,
            out TypeDefinition typeDefinition)
        {
            foreach (var t in module.Types)
            {
                if (t.FullName == algorithmType.FullName)
                {
                    foreach (var m in t.Methods)
                    {
                        if (m.Name == methodName)
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
