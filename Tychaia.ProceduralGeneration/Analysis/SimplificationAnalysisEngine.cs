//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Tychaia.ProceduralGeneration.Analysis.Reporting;
using Tychaia.ProceduralGeneration.Compiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;
using System;
using Tychaia.ProceduralGeneration.Analysis.Output;
using System.Linq;

namespace Tychaia.ProceduralGeneration.Analysis
{
    public class SimplificationAnalysisEngine : AnalysisEngine
    {
        public int WarningLimit { get; set; }
        public int ErrorLimit { get; set; }
        public WarningGradientMode GradientMode { get; set; }

        public enum WarningGradientMode
        {
            BetweenLimits,
            Relative
        }

        public SimplificationAnalysisEngine()
        {
            this.WarningLimit = 5;
            this.ErrorLimit = 20;
            this.GradientMode = WarningGradientMode.BetweenLimits;
        }

        public override void Process(AnalysisLayer layer, ref Analysis.Reporting.Analysis analysis)
        {
            // Create the report and the analysis layer to report on.
            var report = new AnalysisReport();
            report.Name = "Simplification Report";

            // Perform various analysis operations.
            ProcessFrequentExpressions(layer, report);

            // Add our layers and report to the analysis result.
            if (report.Issues.Count > 0)
                layer.Reports.Add(report);
        }

        private void ProcessFrequentExpressions(AnalysisLayer layer, AnalysisReport report)
        {
            // Perform analysis.
            var expressionTreeVisitor = new CalculateExpressionTreeVisitor();
            layer.AstBuilder.CompilationUnit.AcceptVisitor(expressionTreeVisitor);

            // If there are no counts of this being an issue, don't create
            // an issue entry.
            if (expressionTreeVisitor.UniqueNodes.Count(x => x.Value.Count >= this.WarningLimit) == 0)
                return;
            var minCounts = expressionTreeVisitor.UniqueNodes.Where(x => x.Value.Count >= this.WarningLimit).Min(x => x.Value.Count);
            var maxCounts = expressionTreeVisitor.UniqueNodes.Where(x => x.Value.Count >= this.WarningLimit).Max(x => x.Value.Count);
            if (maxCounts == 0)
                return;

            // Write out issue analysis.
            var issue = new AnalysisIssue();
            issue.ID = "W0001";
            issue.Name = "Frequent expressions identified";
            issue.Description = "The following frequent (occurring greater than " + this.WarningLimit + " times) expressions have been identified in the algorithm implementation.";
            foreach (var kv in expressionTreeVisitor.UniqueNodes)
            {
                foreach (var node in kv.Value.AstNodes)
                {
                    var startTrackingInfo = node.Annotations.Where(x => x is StartTrackingInfo).Cast<StartTrackingInfo>().FirstOrDefault();
                    var endTrackingInfo = node.Annotations.Where(x => x is EndTrackingInfo).Cast<EndTrackingInfo>().FirstOrDefault();
                    if (startTrackingInfo == null || endTrackingInfo == null)
                        continue;
                    if (kv.Value.Count < this.WarningLimit)
                        continue;
                    var location = new AnalysisLocationHighlight();
                    location.Start = startTrackingInfo.CharacterPosition;
                    location.End = endTrackingInfo.CharacterPosition;
                    if (this.GradientMode == WarningGradientMode.Relative)
                    {
                        if (maxCounts - Math.Max(this.WarningLimit, minCounts) == 0)
                            location.Importance = 100;
                        else
                            location.Importance = (int)Math.Round(
                                (kv.Value.Count - Math.Max(this.WarningLimit, minCounts)) /
                                (double)(maxCounts - Math.Max(this.WarningLimit, minCounts)) * 100);
                    }
                    else
                    {
                        var count = Math.Min(Math.Max(kv.Value.Count, this.WarningLimit), this.ErrorLimit);
                        location.Importance = (int)(((count - this.WarningLimit) / (double)(this.ErrorLimit - this.WarningLimit)) * 100);
                    }
                    location.Message = "Occurs " + kv.Value.Count + " times";
                    issue.Locations.Add(location);
                }
            }
            issue.FlattenLocations();
            if (issue.Locations.Count == 0)
                return;
            report.Issues.Add(issue);
        }
        
        #region Implementation and AST Visitors
        
        private class AstNodeCount
        {
            public List<AstNode> AstNodes { get; set; }
            public int Count { get; set; }
            
            public AstNodeCount(AstNode initial)
            {
                this.AstNodes = new List<AstNode> { initial };
                this.Count = 1;
            }
        }
        
        private class CalculateExpressionTreeVisitor : DepthFirstAstVisitor
        {
            public Dictionary<string, AstNodeCount> UniqueNodes = new Dictionary<string, AstNodeCount>();
            
            private void AddExpressionToUniqueList(AstNode astNode)
            {
                if (!this.UniqueNodes.ContainsKey(astNode.ToString()))
                    this.UniqueNodes.Add(astNode.ToString(), new AstNodeCount(astNode));
                else
                {
                    this.UniqueNodes[astNode.ToString()].Count += 1;
                    this.UniqueNodes[astNode.ToString()].AstNodes.Add(astNode);
                }
            }
            
            public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
            {
                // No point simplifying ThisReferenceExpressions since they don't
                // cost anything and are optimized out by the CLR.
                if (memberReferenceExpression.Target is ThisReferenceExpression)
                {
                    // Skip it.
                }
                else
                {
                    this.AddExpressionToUniqueList(memberReferenceExpression);
                    base.VisitMemberReferenceExpression(memberReferenceExpression);
                }
            }
            
            public override void VisitInvocationExpression(InvocationExpression invocationExpression)
            {
                // In function invocations, we only want to analyise the arguments as there
                // often isn't any useful simplifications you can do to access the function
                // call.
                foreach (var expr in invocationExpression.Arguments)
                {
                    expr.AcceptVisitor(this);
                }
            }
            
            public override void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
            {
                // We only want to navigate the left hand side expression if it's an
                // indexer expression (e.g. a[b]) and even then we only want to navigate
                // it's arguments.  The developer can't replace the LHS expression with
                // something simpler since in it's nature you need to directly specify
                // what you're assigning into.
                if (assignmentExpression.Left is IndexerExpression)
                {
                    foreach (var expr in (assignmentExpression.Left as IndexerExpression).Arguments)
                    {
                        expr.AcceptVisitor(this);
                    }
                }
                assignmentExpression.Right.AcceptVisitor(this);
            }
            
            public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
            {
                this.AddExpressionToUniqueList(binaryOperatorExpression);
                base.VisitBinaryOperatorExpression(binaryOperatorExpression);
            }
            
            public override void VisitIndexerExpression(IndexerExpression indexerExpression)
            {
                this.AddExpressionToUniqueList(indexerExpression);
                base.VisitIndexerExpression(indexerExpression);
            }
            
            public override void VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression)
            {
                this.AddExpressionToUniqueList(unaryOperatorExpression);
                base.VisitUnaryOperatorExpression(unaryOperatorExpression);
            }
        }
        
        #endregion
    }
}

