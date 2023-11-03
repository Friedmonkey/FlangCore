using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class ForNode : SyntaxNode
    {
        private SyntaxNode initialExpressionNode;
        private SyntaxNode condNode;
        private SyntaxNode stepNode;
        private SyntaxNode block;

        public ForNode(SyntaxNode initialExpressionNode, SyntaxNode condNode, SyntaxNode stepNode, SyntaxNode block) : base(initialExpressionNode.StartPosition, block.EndPosition)
        {
            this.initialExpressionNode = initialExpressionNode;
            this.condNode = condNode;
            this.stepNode = stepNode;
            this.block = block;
        }

        public override NodeType Type => NodeType.For;

        public override FValue Evaluate(Scope scope)
        {
            Scope forScope = new(scope, StartPosition);
            FValue lastVal = FValue.Null;
            initialExpressionNode.Evaluate(forScope);

            while (true)
            {
                if (!condNode.Evaluate(forScope).IsTruthy()) break;
                var forBlockRes = block.Evaluate(forScope);
                if (!forBlockRes.IsNull()) lastVal = forBlockRes;

                if (forScope.State == ScopeState.ShouldBreak) break;
                if (forScope.State == ScopeState.ShouldJump) break;
                if (forScope.State != ScopeState.None) forScope.SetState(ScopeState.None);

                stepNode.Evaluate(forScope);
            }
            if (forScope.State == ScopeState.ShouldBreak)
            { 
                forScope.SetBreakAmount(forScope.BreakAmount-1);
            }
            return lastVal;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return initialExpressionNode;
            yield return condNode;
            yield return stepNode;
            yield return block;
        }
    }
}
