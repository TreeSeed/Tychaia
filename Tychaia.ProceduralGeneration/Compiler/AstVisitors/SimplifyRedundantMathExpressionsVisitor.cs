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
    public class SimplifyRedundantMathExpressionsVisitor : DepthFirstAstVisitor
    {
        public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
        {
            base.VisitBinaryOperatorExpression(binaryOperatorExpression);

            // Looking for patterns like:
            //
            // (x - 2) - x
            var pattern = new BinaryOperatorExpression
            {
                Left = new ParenthesizedExpression
                {
                    Expression = new BinaryOperatorExpression
                    {
                        Left = new NamedNode(
                            "ident", 
                            new IdentifierExpression
                        {
                            Identifier = Pattern.AnyString
                        }),
                        Operator = BinaryOperatorType.Any,
                        Right = new AnyNode("other")
                    }
                },
                Operator = BinaryOperatorType.Any,
                Right = new IdentifierExpressionBackreference("ident")
            };

            if (pattern.IsMatch(binaryOperatorExpression))
            {
                var match = pattern.Match(binaryOperatorExpression);
                var innerOperator = (BinaryOperatorType)((dynamic)binaryOperatorExpression).Left.Expression.Operator;
                var outerOperator = binaryOperatorExpression.Operator;
                switch (outerOperator)
                {
                    case BinaryOperatorType.Subtract:
                        switch (innerOperator)
                        {
                            case BinaryOperatorType.Add:
                                binaryOperatorExpression.ReplaceWith(((AstNode)match.Get("other").First()).Clone());
                                return;
                            case BinaryOperatorType.Subtract:
                                binaryOperatorExpression.ReplaceWith(new UnaryOperatorExpression(
                                    UnaryOperatorType.Minus,
                                    ((Expression)match.Get("other").First()).Clone()));
                                return;
                        }

                        break;
                }
            }

            // Looking for patterns like:
            //
            // (2 - x) + x
            pattern = new BinaryOperatorExpression
            {
                Left = new ParenthesizedExpression
                {
                    Expression = new BinaryOperatorExpression
                    {
                        Left = new AnyNode("other"),
                        Operator = BinaryOperatorType.Any,
                        Right = new NamedNode(
                            "ident", 
                            new IdentifierExpression
                        {
                            Identifier = Pattern.AnyString
                        })
                    }
                },
                Operator = BinaryOperatorType.Any,
                Right = new IdentifierExpressionBackreference("ident")
            };

            if (pattern.IsMatch(binaryOperatorExpression))
            {
                var match = pattern.Match(binaryOperatorExpression);
                var innerOperator = (BinaryOperatorType)((dynamic) binaryOperatorExpression).Left.Expression.Operator;
                var outerOperator = binaryOperatorExpression.Operator;
                switch (outerOperator)
                {
                    case BinaryOperatorType.Add:
                        switch (innerOperator)
                        {
                            case BinaryOperatorType.Subtract:
                                binaryOperatorExpression.ReplaceWith(((AstNode)match.Get("other").First()).Clone());
                                return;
                        }

                        break;
                    case BinaryOperatorType.Subtract:
                        switch (innerOperator)
                        {
                            case BinaryOperatorType.Add:
                                binaryOperatorExpression.ReplaceWith(((AstNode)match.Get("other").First()).Clone());
                                return;
                        }

                        break;
                }
            }

            // Looking for patterns like:
            //
            // (x - (2 + x))
            pattern = new BinaryOperatorExpression
            {
                Left = new NamedNode("ident", new IdentifierExpression
                {
                    Identifier = Pattern.AnyString
                }),
                Operator = BinaryOperatorType.Any,
                Right = new ParenthesizedExpression
                {
                    Expression = new BinaryOperatorExpression
                    {
                        Left = new AnyNode("other"),
                        Operator = BinaryOperatorType.Any,
                        Right = new IdentifierExpressionBackreference("ident")
                    }
                }
            };

            if (pattern.IsMatch(binaryOperatorExpression))
            {
                var match = pattern.Match(binaryOperatorExpression);
                var innerOperator = (BinaryOperatorType)((dynamic)binaryOperatorExpression).Right.Expression.Operator;
                var outerOperator = binaryOperatorExpression.Operator;
                switch (outerOperator)
                {
                    case BinaryOperatorType.Subtract:
                        switch (innerOperator)
                        {
                            case BinaryOperatorType.Add:
                                binaryOperatorExpression.ReplaceWith(((AstNode)match.Get("other").First()).Clone());
                                return;
                        }

                        break;
                }
            }

            // Looking for patterns like:
            //
            // (x - (x - 2))
            pattern = new BinaryOperatorExpression
            {
                Left = new NamedNode(
                    "ident", 
                    new IdentifierExpression
                {
                    Identifier = Pattern.AnyString
                }),
                Operator = BinaryOperatorType.Any,
                Right = new ParenthesizedExpression
                {
                    Expression = new BinaryOperatorExpression
                    {
                        Left = new IdentifierExpressionBackreference("ident"),
                        Operator = BinaryOperatorType.Any,
                        Right = new AnyNode("other")
                    }
                }
            };

            if (pattern.IsMatch(binaryOperatorExpression))
            {
                var match = pattern.Match(binaryOperatorExpression);
                var innerOperator = (BinaryOperatorType)((dynamic) binaryOperatorExpression).Right.Expression.Operator;
                var outerOperator = binaryOperatorExpression.Operator;
                switch (outerOperator)
                {
                    case BinaryOperatorType.Subtract:
                        switch (innerOperator)
                        {
                            case BinaryOperatorType.Subtract:
                                binaryOperatorExpression.ReplaceWith(((AstNode)match.Get("other").First()).Clone());
                                return;
                        }

                        break;
                }
            }
        }
    }
}
