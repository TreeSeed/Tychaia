// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;

namespace Tychaia.ProceduralGeneration.AstVisitors
{
    public class InlineTemporaryCVariablesVisitor : DepthFirstAstVisitor
    {
        private Dictionary<string, Expression> m_CInitializers = new Dictionary<string, Expression>();

        public override void VisitIdentifierExpression(IdentifierExpression identifierExpression)
        {
            base.VisitIdentifierExpression(identifierExpression);

            switch (identifierExpression.Identifier)
            {
                case "__cx":
                case "__cy":
                case "__cz":
                case "__cwidth":
                case "__cheight":
                case "__cdepth":
                    identifierExpression.ReplaceWith(this.m_CInitializers[identifierExpression.Identifier].Clone());
                    break;
            }
        }

        public override void VisitVariableDeclarationStatement(
            VariableDeclarationStatement variableDeclarationStatement)
        {
            base.VisitVariableDeclarationStatement(variableDeclarationStatement);

            if (variableDeclarationStatement.Variables.Count != 1)
                return;

            var variable = variableDeclarationStatement.Variables.First();
            switch (variable.Name)
            {
                case "__cx":
                case "__cy":
                case "__cz":
                case "__cwidth":
                case "__cheight":
                case "__cdepth":
                    this.m_CInitializers[variable.Name] = variable.Initializer;
                    variableDeclarationStatement.Remove();
                    break;
            }
        }
    }
}
