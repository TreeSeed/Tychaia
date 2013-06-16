//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp;
using System.Linq;
using System.Reflection;

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
                base.VisitVariableDeclarationStatement(v);

                // Visit variable initializers.
                foreach (var vv in v.Variables)
                {
                    vv.Initializer.AcceptVisitor(new FindPropertiesVisitor { Algorithm = Algorithm, ParameterContextName = ParameterContextName });
                }
            }

            public override void VisitAnonymousMethodExpression(AnonymousMethodExpression a)
            {
                base.VisitAnonymousMethodExpression(a);

                // Replace properties.
                a.Body.AcceptVisitor(new FindPropertiesVisitor { Algorithm = Algorithm, ParameterContextName = ParameterContextName });
            }

            public override void VisitMemberReferenceExpression(MemberReferenceExpression p)
            {
                base.VisitMemberReferenceExpression(p);

                // Check to see whether this is on the owner of the ProcessCell method.
                if (p.Target is ThisReferenceExpression)
                {
                    // Replace the AST node with the current value.
                    var field = this.Algorithm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Where(v => v.Name == p.MemberName).DefaultIfEmpty(null).First();
                    var prop = this.Algorithm.GetType().GetProperties().Where(v => v.Name == p.MemberName).DefaultIfEmpty(null).First();
                    if (prop == null && field == null)
                        throw new NotSupportedException("Unable to inline ProcessCell methods that invoke other methods on this algorithm.");
                    if (field != null)
                        return;
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

        private class FactorOutAlgorithmFieldsVisitor : DepthFirstAstVisitor
        {
            Dictionary<string, string> m_Mappings;

            public FactorOutAlgorithmFieldsVisitor(Dictionary<string, string> mappings)
            {
                this.m_Mappings = mappings;
            }

            public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
            {
                if (this.m_Mappings.ContainsKey(memberReferenceExpression.MemberName))
                {
                    if (memberReferenceExpression.Target is ThisReferenceExpression)
                    {
                        memberReferenceExpression.ReplaceWith(
                            new IdentifierExpression(
                                this.m_Mappings[memberReferenceExpression.MemberName]));
                        return;
                    }
                }

                base.VisitMemberReferenceExpression(memberReferenceExpression);
            }

            public override void VisitIdentifierExpression(IdentifierExpression identifierExpression)
            {
                if (this.m_Mappings.ContainsKey(identifierExpression.Identifier))
                {
                    identifierExpression.ReplaceWith(
                        new IdentifierExpression(
                            this.m_Mappings[identifierExpression.Identifier]));
                }

                base.VisitIdentifierExpression(identifierExpression);
            }
        }

        public static void FactorOutAlgorithmFields(Type algorithmType, MethodDeclaration method,
                                                    MethodDeclaration initialize, ref string declarations)
        {
            var mappings = new Dictionary<string, string>();
            foreach (var field in algorithmType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.GetCustomAttributes(typeof(FieldForGenerationAttribute), false).Length > 0)
                {
                    mappings.Add(field.Name, "field_" + LayerCompiler.GenerateRandomIdentifier());
                    declarations += field.FieldType.FullName + " " + mappings[field.Name] + ";\r\n";
                }
            }

            var factorOut = new FactorOutAlgorithmFieldsVisitor(mappings);
            method.AcceptVisitor(factorOut);
            if (initialize != null)
                initialize.AcceptVisitor(factorOut);
        }

        /// <summary>
        /// Refactors the names of parameters and their references so the
        /// method body can be copied directly into the output.
        /// </summary>
        public static void InlineMethod(IAlgorithm algorithm, MethodDeclaration method, string outputName, string[] inputNames,
                                        Expression x, Expression y, Expression z,
                                        Expression i, Expression j, Expression k,
                                        Expression width, Expression height, Expression depth,
                                        Expression ox, Expression oy, Expression oz)
        {
            if (algorithm == null) throw new ArgumentNullException("algorithm");
            if (method == null) throw new ArgumentNullException("method");
            if (outputName == null) throw new ArgumentNullException("outputName");
            if (inputNames == null) throw new ArgumentNullException("inputNames");
            if (x == null) throw new ArgumentNullException("x");
            if (y == null) throw new ArgumentNullException("y");
            if (z == null) throw new ArgumentNullException("z");
            if (i == null) throw new ArgumentNullException("i");
            if (j == null) throw new ArgumentNullException("j");
            if (k == null) throw new ArgumentNullException("k");
            if (width == null) throw new ArgumentNullException("width");
            if (height == null) throw new ArgumentNullException("height");
            if (depth == null) throw new ArgumentNullException("depth");
            if (ox == null) throw new ArgumentNullException("ox");
            if (oy == null) throw new ArgumentNullException("oy");
            if (oz == null) throw new ArgumentNullException("oz");

            var parameterContext = method.Parameters.ElementAt(0);
            var parameterInputs = new ParameterDeclaration[method.Parameters.Count - 14];
            for (var idx = 1; idx < method.Parameters.Count - 13; idx++)
                parameterInputs[idx - 1] = method.Parameters.ElementAt(idx);
            var parameterOutput = method.Parameters.Reverse().ElementAt(12);
            var parameterX = method.Parameters.Reverse().ElementAt(11);
            var parameterY = method.Parameters.Reverse().ElementAt(10);
            var parameterZ = method.Parameters.Reverse().ElementAt(9);
            var parameterI = method.Parameters.Reverse().ElementAt(8);
            var parameterJ = method.Parameters.Reverse().ElementAt(7);
            var parameterK = method.Parameters.Reverse().ElementAt(6);
            var parameterWidth = method.Parameters.Reverse().ElementAt(5);
            var parameterHeight = method.Parameters.Reverse().ElementAt(4);
            var parameterDepth = method.Parameters.Reverse().ElementAt(3);
            var parameterOX = method.Parameters.Reverse().ElementAt(2);
            var parameterOY = method.Parameters.Reverse().ElementAt(1);
            var parameterOZ = method.Parameters.Reverse().ElementAt(0);

            // Replace properties.
            method.AcceptVisitor(new FindPropertiesVisitor { Algorithm = algorithm, ParameterContextName = parameterContext.Name });

            // Replace identifiers.
            var identifiers =
                method
                    .Body
                    .Descendants
                    .Where(v => v is IdentifierExpression)
                    .Cast<IdentifierExpression>()
                    .ToArray();
            foreach (var e in identifiers)
            {
                if (e.Identifier == parameterX.Name)
                    e.ReplaceWith(x.Clone());
                else if (e.Identifier == parameterY.Name)
                    e.ReplaceWith(y.Clone());
                else if (e.Identifier == parameterZ.Name)
                    e.ReplaceWith(z.Clone());
                else if (e.Identifier == parameterI.Name)
                    e.ReplaceWith(i.Clone());
                else if (e.Identifier == parameterJ.Name)
                    e.ReplaceWith(j.Clone());
                else if (e.Identifier == parameterK.Name)
                    e.ReplaceWith(k.Clone());
                else if (e.Identifier == parameterWidth.Name)
                    e.ReplaceWith(width.Clone());
                else if (e.Identifier == parameterHeight.Name)
                    e.ReplaceWith(height.Clone());
                else if (e.Identifier == parameterDepth.Name)
                    e.ReplaceWith(depth.Clone());
                else if (e.Identifier == parameterOX.Name)
                    e.ReplaceWith(ox.Clone());
                else if (e.Identifier == parameterOY.Name)
                    e.ReplaceWith(oy.Clone());
                else if (e.Identifier == parameterOZ.Name)
                    e.ReplaceWith(oz.Clone());
                else if (e.Identifier == parameterOutput.Name)
                    e.Identifier = outputName;
                else if (parameterInputs.Count(v => v.Name == e.Identifier) > 0)
                    e.Identifier = inputNames.ElementAt(Array.FindIndex(parameterInputs, v => v.Name == e.Identifier));
                else if (e.Identifier == "context")
                    e.ReplaceWith(new ThisReferenceExpression());
            }
        }

        /// <summary>
        /// Refactors the names of parameters and their references so the
        /// method body can be copied directly into the output.
        /// </summary>
        public static void InlineInitialize(IAlgorithm algorithm, MethodDeclaration method)
        {
            if (algorithm == null) throw new ArgumentNullException("algorithm");
            if (method == null) throw new ArgumentNullException("method");

            var parameterContext = method.Parameters.ElementAt(0);

            // Replace properties.
            method.AcceptVisitor(new FindPropertiesVisitor { Algorithm = algorithm, ParameterContextName = parameterContext.Name });

            // Replace identifiers.
            foreach (var i in method.Body.Descendants.Where(v => v is IdentifierExpression).Cast<IdentifierExpression>())
            {
                if (i.Identifier == "context")
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
