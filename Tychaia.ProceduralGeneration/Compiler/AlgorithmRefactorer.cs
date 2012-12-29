//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;
using System.Linq;

namespace Tychaia.ProceduralGeneration.Compiler
{
    public static class AlgorithmRefactorer
    {
        /// <summary>
        /// Refactors the names of parameters and their references so the
        /// method body can be copied directly into the output.
        /// </summary>
        public static void InlineMethod(IAlgorithm algorithm, MethodDeclaration method, string outputName, string[] inputNames)
        {
            var parameterContext = method.Parameters.ElementAt(0);
            var parameterOutput = method.Parameters.ElementAt(1);
            var parameterInputs = new ParameterDeclaration[method.Parameters.Count - 11];
            for (var i = 2; i < method.Parameters.Count - 9; i++)
                parameterInputs[i - 2] = method.Parameters.ElementAt(i);
            var parameterX = method.Parameters.Reverse().ElementAt(8);
            var parameterY = method.Parameters.Reverse().ElementAt(7);
            var parameterZ = method.Parameters.Reverse().ElementAt(6);
            var parameterI = method.Parameters.Reverse().ElementAt(5);
            var parameterJ = method.Parameters.Reverse().ElementAt(4);
            var parameterK = method.Parameters.Reverse().ElementAt(3);
            var parameterWidth = method.Parameters.Reverse().ElementAt(2);
            var parameterHeight = method.Parameters.Reverse().ElementAt(1);
            var parameterDepth = method.Parameters.Reverse().ElementAt(0);

            // Replace properties.
            foreach (var p in method.Body.Descendants.Where(v => v is MemberReferenceExpression).Cast<MemberReferenceExpression>())
            {
                // Check to see whether this is on the owner of the ProcessCell method.
                if (p.Target is ThisReferenceExpression)
                {
                    // Check to see whether it is a property or method call.
                    if (p.Parent is InvocationExpression)
                    {
                        // This is a method call.
                        throw new NotSupportedException("Unable to inline ProcessCell methods that invoke other methods on this algorithm.");
                    }
                    else
                    {
                        // Replace the AST node with the current value.
                        var prop = algorithm.GetType().GetProperties().Where(v => v.Name == p.MemberName).First();
                        p.ReplaceWith(new PrimitiveExpression(prop.GetGetMethod().Invoke(algorithm, null)));
                    }
                }
                else if (p.Target is IdentifierExpression && (p.Target as IdentifierExpression).Identifier == parameterContext.Name)
                {
                    // This is a reference to the runtime context, which we replace with this.
                    p.Target.ReplaceWith(new ThisReferenceExpression());
                }
            }

            // Replace identifiers.
            foreach (var i in method.Body.Descendants.Where(v => v is IdentifierExpression).Cast<IdentifierExpression>())
            {
                if (i.Identifier == parameterX.Name)
                    i.ReplaceWith(
                        new ParenthesizedExpression(
                            new BinaryOperatorExpression(
                                new IdentifierExpression("x"),
                                BinaryOperatorType.Add,
                                new IdentifierExpression("i")
                    )
                    ));
                else if (i.Identifier == parameterY.Name)
                    i.ReplaceWith(
                        new ParenthesizedExpression(
                            new BinaryOperatorExpression(
                                new IdentifierExpression("y"),
                                BinaryOperatorType.Add,
                                new IdentifierExpression("j")
                    )
                    ));
                else if (i.Identifier == parameterZ.Name)
                    i.ReplaceWith(
                        new ParenthesizedExpression(
                            new BinaryOperatorExpression(
                                new IdentifierExpression("z"),
                                BinaryOperatorType.Add,
                                new IdentifierExpression("k")
                    )
                    ));
                else if (i.Identifier == parameterI.Name)
                    i.Identifier = "i";
                else if (i.Identifier == parameterJ.Name)
                    i.Identifier = "j";
                else if (i.Identifier == parameterK.Name)
                    i.Identifier = "k";
                else if (i.Identifier == parameterWidth.Name)
                    i.Identifier = "width";
                else if (i.Identifier == parameterHeight.Name)
                    i.Identifier = "height";
                else if (i.Identifier == parameterDepth.Name)
                    i.Identifier = "depth";
                else if (i.Identifier == parameterOutput.Name)
                    i.Identifier = outputName;
                else if (parameterInputs.Count(v => v.Name == i.Identifier) > 0)
                    i.Identifier = inputNames.ElementAt(Array.FindIndex(parameterInputs, v => v.Name == i.Identifier));
            }
        }

        /// <summary>
        /// Removes the using statements in the compilation unit and moves them into
        /// a list so they can be added into the compiled layer template at a different
        /// area.
        /// </summary>
        public static void RemoveUsingStatements(CompilationUnit unit, List<string> output)
        {
            foreach (var u in unit.Children)
            {
                Console.WriteLine("--- " + u);
            }

            var usings = unit.Descendants.Where(v => v is UsingDeclaration).Cast<UsingDeclaration>();
            foreach (var u in usings)
            {
                output.Add(u.Import.GetText());
                u.Remove();
            }
        }
    }
}