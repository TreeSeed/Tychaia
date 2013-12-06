// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using ICSharpCode.NRefactory.CSharp;

namespace Tychaia.ProceduralGeneration.AstVisitors
{
    public class SimplifyConstantMathExpressionsVisitor : DepthFirstAstVisitor
    {
        public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
        {
            base.VisitBinaryOperatorExpression(binaryOperatorExpression);

            // Looking for patterns like:
            //
            // 2 - 2
            var valueLeft = AstHelpers.GetValueFromExpression(binaryOperatorExpression.Left);
            var valueRight = AstHelpers.GetValueFromExpression(binaryOperatorExpression.Right);
            if (valueLeft == null || valueRight == null)
                return;

            try
            {
                dynamic valueResult;
                switch (binaryOperatorExpression.Operator)
                {
                    case BinaryOperatorType.Add:
                        valueResult = valueLeft + valueRight;
                        break;
                    case BinaryOperatorType.Subtract:
                        valueResult = valueLeft - valueRight;
                        break;
                    case BinaryOperatorType.Divide:
                        valueResult = valueLeft / valueRight;
                        break;
                    case BinaryOperatorType.Multiply:
                        valueResult = valueLeft * valueRight;
                        break;
                    default:
                        return;
                }

                binaryOperatorExpression.ReplaceWith(new PrimitiveExpression(valueResult));
            }
            catch (Exception)
            {
                // Can't do anything with this value perhaps, so just ignore it and leave
                // the code as-is.
            }
        }
    }
}
