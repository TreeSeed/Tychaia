// 
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
// 
using ICSharpCode.NRefactory.CSharp;

namespace Tychaia.ProceduralGeneration.AstVisitors
{
    public static class AstHelpers
    {
        public static dynamic GetValueFromParenthesizedExpression(ParenthesizedExpression expr)
        {
            return GetValueFromExpression(expr.Expression);
        }

        public static dynamic GetValueFromUnaryExpression(UnaryOperatorExpression expr)
        {
            var value = GetValueFromExpression(expr.Expression);
            if (value == null)
                return null;

            switch (expr.Operator)
            {
                case UnaryOperatorType.Not:
                    return !value;
                case UnaryOperatorType.Minus:
                    return -value;
                case UnaryOperatorType.Plus:
                    return +value;
                default:
                    return null;
            }
        }

        public static dynamic GetValueFromExpression(Expression expr)
        {
            if (expr is PrimitiveExpression)
                return ((PrimitiveExpression) expr).Value;
            if (expr is UnaryOperatorExpression)
                return GetValueFromUnaryExpression((UnaryOperatorExpression) expr);
            if (expr is ParenthesizedExpression)
                return GetValueFromParenthesizedExpression((ParenthesizedExpression) expr);
            return null;
        }

        private static DepthFirstAstVisitor[] GetVisitors()
        {
            return new DepthFirstAstVisitor[]
            {
                new RemoveRedundantPrimitiveCastsVisitor(),
                new RemoveParenthesisVisitor(),
                new SimplifyConstantMathExpressionsVisitor(),
                new SimplifyCombinedMathExpressionsVisitor(),
                new SimplifyZeroAndConditionalExpressionsVisitor(),
                new SimplifyRedundantMathExpressionsVisitor()
            };
        }

        public static void OptimizeCompilationUnit(CompilationUnit tree)
        {
            tree.AcceptVisitor(new InlineTemporaryCVariablesVisitor());
            var visitors = GetVisitors();
            string oldText = null;
            while (tree.GetText() != oldText)
            {
                oldText = tree.GetText();
                foreach (var visitor in visitors)
                    tree.AcceptVisitor(visitor);
            }
        }

        public static Expression OptimizeExpression(Expression node)
        {
            var root = new ParenthesizedExpression(node.Clone());
            var visitors = GetVisitors();
            string oldText = null;
            while (root.GetText() != oldText)
            {
                oldText = root.GetText();
                foreach (var visitor in visitors)
                    root.Expression.AcceptVisitor(visitor);
            }
            return root.Expression;
        }
    }
}