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
                return ((PrimitiveExpression)expr).Value;
            if (expr is UnaryOperatorExpression)
                return GetValueFromUnaryExpression((UnaryOperatorExpression)expr);
            if (expr is ParenthesizedExpression)
                return GetValueFromParenthesizedExpression((ParenthesizedExpression)expr);
            return null;
        }
    }
}

