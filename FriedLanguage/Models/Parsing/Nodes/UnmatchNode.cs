using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class UnmatchNode : SyntaxNode
    {
        public SyntaxNode NumExpr { get; set; } = null;

        public override NodeType Type => NodeType.Unmatch;

        public UnmatchNode(SyntaxToken unmatchToken) : base(unmatchToken.Position, unmatchToken.EndPosition) { }
        public UnmatchNode(SyntaxToken unmatchToken, SyntaxNode numExpr) : base(unmatchToken.Position, unmatchToken.EndPosition)
        {
            this.NumExpr = numExpr;
        }

        public override FValue Evaluate(Scope scope)
        {
            //scope.SetState(ScopeState.ShouldUnmatch);
            //return FValue.Null;
            int unmatchAmount = 1;
            if (NumExpr != null)
            {
                var v = NumExpr.Evaluate(scope);
                if (v is FInt fint)
                    unmatchAmount = fint.Value;
                else
                    throw new Exception("Expected int for amount");
            }
            scope.SetState(ScopeState.ShouldUnmatch);
            scope.SetUnmatchAmount(unmatchAmount);
            return FValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString()
        {
            return "UnmatchNode:";
        }
    }
}
