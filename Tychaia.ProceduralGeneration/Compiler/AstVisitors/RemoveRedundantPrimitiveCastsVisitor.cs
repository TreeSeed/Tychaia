//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace Tychaia.ProceduralGeneration.AstVisitors
{
    public class RemoveRedundantPrimitiveCastsVisitor : DepthFirstAstVisitor
    {
        public override void VisitCastExpression(CastExpression castExpression)
        {
            base.VisitCastExpression(castExpression);

            var expression = castExpression.Expression;
            if (expression is ParenthesizedExpression)
                expression = (expression as ParenthesizedExpression).Expression;

            object value = null;
            if (expression is PrimitiveExpression)
                value = (expression as PrimitiveExpression).Value;
            else if (expression is UnaryOperatorExpression &&
                     (expression as UnaryOperatorExpression).Expression is PrimitiveExpression)
            {
                var primitive = (expression as UnaryOperatorExpression).Expression as PrimitiveExpression;
                value = primitive.Value;
            }

            if (value != null)
            {
                var type = (castExpression.Type as PrimitiveType).KnownTypeCode;
                if ((type == KnownTypeCode.Int16 && value is Int16) ||
                    (type == KnownTypeCode.Int32 && value is Int32) ||
                    (type == KnownTypeCode.Int64 && value is Int64) ||
                    (type == KnownTypeCode.UInt16 && value is UInt16) ||
                    (type == KnownTypeCode.UInt32 && value is UInt32) ||
                    (type == KnownTypeCode.UInt64 && value is UInt64) ||
                    (type == KnownTypeCode.Double && value is Double) ||
                    (type == KnownTypeCode.Single && value is Single) ||
                    (type == KnownTypeCode.String && value is String) ||
                    (type == KnownTypeCode.Boolean && value is Boolean) ||
                    (type == KnownTypeCode.Char && value is Char) ||
                    (type == KnownTypeCode.Byte && value is Byte) ||
                    (type == KnownTypeCode.SByte && value is SByte) ||
                    (type == KnownTypeCode.Decimal && value is Decimal))
                {
                    castExpression.ReplaceWith(expression);
                    return;
                }
            }
        }
    }
}

