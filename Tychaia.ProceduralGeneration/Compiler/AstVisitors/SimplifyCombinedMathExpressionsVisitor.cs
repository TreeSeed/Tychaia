// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;

namespace Tychaia.ProceduralGeneration.AstVisitors
{
    public class SimplifyCombinedMathExpressionsVisitor : DepthFirstAstVisitor
    {
        public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
        {
            base.VisitBinaryOperatorExpression(binaryOperatorExpression);

            // Looking for patterns like:
            //
            // (x - 2) - 2
            // (x / 2) / 2
            // (x - 2) + 4
            // (x * 2) / 3
            var pattern = new BinaryOperatorExpression
            {
                Left = new ParenthesizedExpression
                {
                    Expression = new BinaryOperatorExpression
                    {
                        Left = new AnyNode("left"),
                        Operator = BinaryOperatorType.Any,
                        Right = new AnyNode("rightA")
                    }
                },
                Operator = BinaryOperatorType.Any,
                Right = new AnyNode("rightB")
            };

            if (binaryOperatorExpression.Operator != BinaryOperatorType.Add &&
                binaryOperatorExpression.Operator != BinaryOperatorType.Subtract &&
                binaryOperatorExpression.Operator != BinaryOperatorType.Multiply &&
                binaryOperatorExpression.Operator != BinaryOperatorType.Divide)
            {
                return;
            }

            if (pattern.IsMatch(binaryOperatorExpression))
            {
                var match = pattern.Match(binaryOperatorExpression);
                var outerOperator = binaryOperatorExpression.Operator;
                var innerOperator = (BinaryOperatorType)((dynamic)binaryOperatorExpression.Left).Expression.Operator;
                var innerValue = AstHelpers.GetValueFromExpression((Expression)match.Get("rightA").First());
                var outerValue = AstHelpers.GetValueFromExpression((Expression)match.Get("rightB").First());
                if (innerValue == null && outerValue == null)
                    return;

                // If the operators are equal, then we handle expression like (x - 2) - 4.
                if (innerOperator == outerOperator)
                {
                    dynamic resultValue;
                    switch (binaryOperatorExpression.Operator)
                    {
                        case BinaryOperatorType.Add:
                        case BinaryOperatorType.Subtract:
                            resultValue = innerValue + outerValue;
                            break;
                        case BinaryOperatorType.Multiply:
                        case BinaryOperatorType.Divide:
                            resultValue = innerValue * outerValue;
                            break;
                        default:
                            return;
                    }

                    binaryOperatorExpression.ReplaceWith(new BinaryOperatorExpression(
                        (match.Get("left").First() as Expression).Clone(),
                        binaryOperatorExpression.Operator,
                        new PrimitiveExpression(resultValue)));
                    return;
                }

                // Otherwise handle the slightly more complex cases.
                PrimitiveExpression result = null;
                switch (innerOperator)
                {
                    case BinaryOperatorType.Add:
                        switch (outerOperator)
                        {
                            case BinaryOperatorType.Subtract:
                                result = new PrimitiveExpression(innerValue - outerValue);
                                break;
                        }

                        break;
                    case BinaryOperatorType.Subtract:
                        switch (outerOperator)
                        {
                            case BinaryOperatorType.Add:
                                result = new PrimitiveExpression(innerValue - outerValue);
                                break;
                        }

                        break;
                }

                if (result != null)
                {
                    binaryOperatorExpression.ReplaceWith(new BinaryOperatorExpression(
                        (match.Get("left").First() as Expression).Clone(),
                        binaryOperatorExpression.Operator,
                        result));
                }
            }
        }
    }
}
