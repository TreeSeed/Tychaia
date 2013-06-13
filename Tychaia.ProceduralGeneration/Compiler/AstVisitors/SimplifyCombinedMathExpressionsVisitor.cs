//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
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

            var pattern = new BinaryOperatorExpression {
                Left = new ParenthesizedExpression {
                    Expression = new BinaryOperatorExpression {
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
                if (binaryOperatorExpression.Operator ==
                    ((binaryOperatorExpression.Left as ParenthesizedExpression).Expression
                    as BinaryOperatorExpression).Operator)
                {
                    if (match.Get("rightA").First() is PrimitiveExpression &&
                        match.Get("rightB").First() is PrimitiveExpression)
                    {
                        var aValue = (match.Get("rightA").First() as PrimitiveExpression).Value;
                        var bValue = (match.Get("rightB").First() as PrimitiveExpression).Value;
                        if ((aValue is int || aValue is long || aValue is double || aValue is float) &&
                            (bValue is int || bValue is long || bValue is double || bValue is float))
                        {
                            object resultValue;
                            switch (binaryOperatorExpression.Operator)
                            {
                                case BinaryOperatorType.Add:
                                case BinaryOperatorType.Subtract:
                                    resultValue = (object)((dynamic)aValue + (dynamic)bValue);
                                    break;
                                case BinaryOperatorType.Multiply:
                                case BinaryOperatorType.Divide:
                                    resultValue = (object)((dynamic)aValue * (dynamic)bValue);
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
                    }
                }
            }
        }
    }
}

