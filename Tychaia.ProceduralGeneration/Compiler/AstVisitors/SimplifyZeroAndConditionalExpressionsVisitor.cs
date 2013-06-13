//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;

namespace Tychaia.ProceduralGeneration.AstVisitors
{
    public class SimplifyZeroAndConditionalExpressionsVisitor : DepthFirstAstVisitor
    {
        private bool ReplaceConditionalAnd(AstNode root, Expression a, AstNode b)
        {
            bool? value = null;
            if (a is PrimitiveExpression &&
                (a as PrimitiveExpression).Value is bool)
            {
                value = (bool)(a as PrimitiveExpression).Value;
            }
            if (a is UnaryOperatorExpression &&
                (a as UnaryOperatorExpression).Operator == UnaryOperatorType.Not &&
                (a as UnaryOperatorExpression).Expression is PrimitiveExpression &&
                (((a as UnaryOperatorExpression).Expression as PrimitiveExpression).Value) is bool)
            {
                value = !(bool)(((a as UnaryOperatorExpression).Expression as PrimitiveExpression).Value);
            }
            if (value != null)
            {
                if ((bool)value)
                    root.ReplaceWith(b);
                else
                    root.ReplaceWith(new PrimitiveExpression(false));
                return true;
            }
            return false;
        }

        private bool ReplaceConditionalOr(AstNode root, Expression a, AstNode b)
        {
            bool? value = null;
            if (a is PrimitiveExpression &&
                (a as PrimitiveExpression).Value is bool)
            {
                value = (bool)(a as PrimitiveExpression).Value;
            }
            if (a is UnaryOperatorExpression &&
                (a as UnaryOperatorExpression).Operator == UnaryOperatorType.Not &&
                (a as UnaryOperatorExpression).Expression is PrimitiveExpression &&
                (((a as UnaryOperatorExpression).Expression as PrimitiveExpression).Value) is bool)
            {
                value = !(bool)(((a as UnaryOperatorExpression).Expression as PrimitiveExpression).Value);
            }
            if (value != null)
            {
                if ((bool)value)
                    root.ReplaceWith(new PrimitiveExpression(true));
                else
                    root.ReplaceWith(b);
                return true;
            }
            return false;
        }

        public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
        {
            base.VisitBinaryOperatorExpression(binaryOperatorExpression);

            var left = binaryOperatorExpression.Left;
            var right = binaryOperatorExpression.Right;

            if (left is IdentifierExpression &&
                right is IdentifierExpression)
            {
                var leftIdent = (left as IdentifierExpression).Identifier;
                var rightIdent = (right as IdentifierExpression).Identifier;
                if (leftIdent == rightIdent)
                {
                    if (binaryOperatorExpression.Operator == BinaryOperatorType.Subtract)
                    {
                        binaryOperatorExpression.ReplaceWith(new PrimitiveExpression(0));
                        return;
                    }
                }
            }

            if (right is PrimitiveExpression &&
                (right as PrimitiveExpression).Value is int &&
                (int)((right as PrimitiveExpression).Value) == 0)
            {
                if (binaryOperatorExpression.Operator == BinaryOperatorType.Add ||
                    binaryOperatorExpression.Operator == BinaryOperatorType.Subtract)
                {
                    binaryOperatorExpression.ReplaceWith(left);
                    return;
                }
            }

            if (binaryOperatorExpression.Operator == BinaryOperatorType.ConditionalAnd)
            {
                if (ReplaceConditionalAnd(binaryOperatorExpression, left, right) ||
                    ReplaceConditionalAnd(binaryOperatorExpression, right, left))
                    return;
            }

            if (binaryOperatorExpression.Operator == BinaryOperatorType.ConditionalOr)
            {
                if (ReplaceConditionalOr(binaryOperatorExpression, left, right) ||
                    ReplaceConditionalOr(binaryOperatorExpression, right, left))
                    return;
            }
        }

        public override void VisitIfElseStatement(IfElseStatement ifElseStatement)
        {
            base.VisitIfElseStatement(ifElseStatement);

            if (ifElseStatement.Condition is PrimitiveExpression &&
                (ifElseStatement.Condition as PrimitiveExpression).Value is bool)
            {
                var value = (bool)(ifElseStatement.Condition as PrimitiveExpression).Value;
                if (value)
                    ifElseStatement.ReplaceWith(ifElseStatement.TrueStatement);
                else
                    ifElseStatement.ReplaceWith(ifElseStatement.FalseStatement);
                return;
            }
        }

        public override void VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression)
        {
            base.VisitParenthesizedExpression(parenthesizedExpression);

            if (parenthesizedExpression.Expression is PrimitiveExpression)
                parenthesizedExpression.ReplaceWith(parenthesizedExpression.Expression);
            if (parenthesizedExpression.Expression is IdentifierExpression)
                parenthesizedExpression.ReplaceWith(parenthesizedExpression.Expression);
            if (parenthesizedExpression.Expression is ParenthesizedExpression)
                parenthesizedExpression.ReplaceWith(parenthesizedExpression.Expression);
        }

        public override void VisitBlockStatement(BlockStatement blockStatement)
        {
            base.VisitBlockStatement(blockStatement);

            if (blockStatement.Statements.Count == 1)
            {
                try
                {
                    blockStatement.ReplaceWith(blockStatement.Statements.ElementAt(0));
                }
                catch (ArgumentException)
                {
                    // Can't simplify this...
                }
            }
        }
    }
}

