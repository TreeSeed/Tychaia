// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
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
                if ((type == KnownTypeCode.Int16 && value is short) ||
                    (type == KnownTypeCode.Int32 && value is int) ||
                    (type == KnownTypeCode.Int64 && value is long) ||
                    (type == KnownTypeCode.UInt16 && value is ushort) ||
                    (type == KnownTypeCode.UInt32 && value is uint) ||
                    (type == KnownTypeCode.UInt64 && value is ulong) ||
                    (type == KnownTypeCode.Double && value is double) ||
                    (type == KnownTypeCode.Single && value is float) ||
                    (type == KnownTypeCode.String && value is string) ||
                    (type == KnownTypeCode.Boolean && value is bool) ||
                    (type == KnownTypeCode.Char && value is char) ||
                    (type == KnownTypeCode.Byte && value is byte) ||
                    (type == KnownTypeCode.SByte && value is sbyte) ||
                    (type == KnownTypeCode.Decimal && value is decimal))
                {
                    castExpression.ReplaceWith(expression);
                }
            }
        }
    }
}
