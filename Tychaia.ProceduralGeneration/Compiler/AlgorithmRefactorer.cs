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
        /// Visitor that fixes up properties.
        /// </summary>
        private class FindPropertiesVisitor : DepthFirstAstVisitor
        {
            public IAlgorithm Algorithm;
            public string ParameterContextName;

            public override void VisitVariableDeclarationStatement(VariableDeclarationStatement v)
            {
                // Visit variable initializers.
                foreach (var vv in v.Variables)
                {
                    vv.Initializer.AcceptVisitor(new FindPropertiesVisitor { Algorithm = Algorithm, ParameterContextName = ParameterContextName });
                }
            }

            public override void VisitAnonymousMethodExpression(AnonymousMethodExpression a)
            {
                // Replace properties.
                a.Body.AcceptVisitor(new FindPropertiesVisitor { Algorithm = Algorithm, ParameterContextName = ParameterContextName });
            }

            public override void VisitMemberReferenceExpression(MemberReferenceExpression p)
            {
                // Check to see whether this is on the owner of the ProcessCell method.
                if (p.Target is ThisReferenceExpression)
                {
                    // Replace the AST node with the current value.
                    var prop = this.Algorithm.GetType().GetProperties().Where(v => v.Name == p.MemberName).DefaultIfEmpty(null).First();
                    if (prop == null)
                        throw new NotSupportedException("Unable to inline ProcessCell methods that invoke other methods on this algorithm.");
                    var value = prop.GetGetMethod().Invoke(this.Algorithm, null);
                    if (value is Enum)
                        p.ReplaceWith(new PrimitiveExpression(value, value.GetType().FullName.Replace("+", ".") + "." + value));
                    else
                        p.ReplaceWith(new PrimitiveExpression(value));
                }
                else if (p.Target is IdentifierExpression && (p.Target as IdentifierExpression).Identifier == ParameterContextName)
                {
                    // This is a reference to the runtime context, which we replace with this.
                    p.Target.ReplaceWith(new ThisReferenceExpression());
                }
            }
        }

        /// <summary>
        /// Refactors the names of parameters and their references so the
        /// method body can be copied directly into the output.
        /// </summary>
        public static void InlineMethod(IAlgorithm algorithm, MethodDeclaration method, string outputName, string[] inputNames,
                                        string xStartOffset, string yStartOffset, string zStartOffset,
                                        string width, string height, string depth)
        {
            var parameterContext = method.Parameters.ElementAt(0);
            var parameterInputs = new ParameterDeclaration[method.Parameters.Count - 11];
            for (var i = 1; i < method.Parameters.Count - 10; i++)
                parameterInputs[i - 1] = method.Parameters.ElementAt(i);
            var parameterOutput = method.Parameters.Reverse().ElementAt(9);
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
            method.AcceptVisitor(new FindPropertiesVisitor { Algorithm = algorithm, ParameterContextName = parameterContext.Name });

            // Replace identifiers.
            foreach (var i in method.Body.Descendants.Where(v => v is IdentifierExpression).Cast<IdentifierExpression>())
            {
                if (i.Identifier == parameterX.Name)
                    i.ReplaceWith(
                        new ParenthesizedExpression(
                        new BinaryOperatorExpression(
                        new IdentifierExpression("x"),
                        BinaryOperatorType.Add,
                        new BinaryOperatorExpression(
                        new IdentifierExpression("i"),
                        BinaryOperatorType.Add,
                        new IdentifierExpression(xStartOffset)
                    ))));
                else if (i.Identifier == parameterY.Name)
                    i.ReplaceWith(
                        new ParenthesizedExpression(
                        new BinaryOperatorExpression(
                        new IdentifierExpression("y"),
                        BinaryOperatorType.Add,
                        new BinaryOperatorExpression(
                        new IdentifierExpression("j"),
                        BinaryOperatorType.Add,
                        new IdentifierExpression(yStartOffset)
                    ))));
                else if (i.Identifier == parameterZ.Name)
                    i.ReplaceWith(
                        new ParenthesizedExpression(
                        new BinaryOperatorExpression(
                        new IdentifierExpression("z"),
                        BinaryOperatorType.Add,
                        new BinaryOperatorExpression(
                        new IdentifierExpression("k"),
                        BinaryOperatorType.Add,
                        new IdentifierExpression(zStartOffset)
                    ))));
                else if (i.Identifier == parameterI.Name)
                    i.ReplaceWith(
                        new ParenthesizedExpression(
                        new BinaryOperatorExpression(
                        new IdentifierExpression("i"),
                        BinaryOperatorType.Add,
                        new IdentifierExpression(xStartOffset)
                    )));
                else if (i.Identifier == parameterJ.Name)
                    i.ReplaceWith(
                        new ParenthesizedExpression(
                        new BinaryOperatorExpression(
                        new IdentifierExpression("j"),
                        BinaryOperatorType.Add,
                        new IdentifierExpression(yStartOffset)
                    )));
                else if (i.Identifier == parameterK.Name)
                    i.ReplaceWith(
                        new ParenthesizedExpression(
                        new BinaryOperatorExpression(
                        new IdentifierExpression("k"),
                        BinaryOperatorType.Add,
                        new IdentifierExpression(zStartOffset)
                    )));
                else if (i.Identifier == parameterWidth.Name)
                    i.Identifier = width;
                else if (i.Identifier == parameterHeight.Name)
                    i.Identifier = height;
                else if (i.Identifier == parameterDepth.Name)
                    i.Identifier = depth;
                else if (i.Identifier == parameterOutput.Name)
                    i.Identifier = outputName;
                else if (parameterInputs.Count(v => v.Name == i.Identifier) > 0)
                    i.Identifier = inputNames.ElementAt(Array.FindIndex(parameterInputs, v => v.Name == i.Identifier));
                else if (i.Identifier == "context")
                    i.ReplaceWith(new ThisReferenceExpression());
            }
        }

        /// <summary>
        /// Removes the using statements in the compilation unit and moves them into
        /// a list so they can be added into the compiled layer template at a different
        /// area.
        /// </summary>
        public static void RemoveUsingStatements(CompilationUnit unit, List<string> output)
        {
            var usings = unit.Descendants.Where(v => v is UsingDeclaration).Cast<UsingDeclaration>();
            foreach (var u in usings)
            {
                output.Add(u.Import.GetText());
                u.Remove();
            }
        }
    }
}
