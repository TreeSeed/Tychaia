// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using ICSharpCode.NRefactory.CSharp;

namespace Tychaia.ProceduralGeneration.AstVisitors
{
    public class RemoveParenthesisVisitor : DepthFirstAstVisitor
    {
        public override void VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression)
        {
            base.VisitParenthesizedExpression(parenthesizedExpression);

            if (parenthesizedExpression.Expression is PrimitiveExpression)
                parenthesizedExpression.ReplaceWith(parenthesizedExpression.Expression);
        }
    }
}
