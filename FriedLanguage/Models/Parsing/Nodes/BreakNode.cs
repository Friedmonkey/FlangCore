using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class BreakNode : SyntaxNode
    {
        public SyntaxNode NumExpr { get; set; } = null;
        public override NodeType Type => NodeType.Break;

        public BreakNode(SyntaxToken breakToken) : base(breakToken.Position, breakToken.EndPosition) { }
        public BreakNode(SyntaxToken breakToken,SyntaxNode numExpr) : base(breakToken.Position, breakToken.EndPosition) 
        {
            this.NumExpr = numExpr;
        }

        public override FValue Evaluate(Scope scope)
        {
            int breakAmount = 1;
            if (NumExpr != null)
            { 
                var v = NumExpr.Evaluate(scope);
                if (v is FInt fint)
                    breakAmount = fint.Value;
                else
                    throw new Exception("Expected int for amount");
            }
            scope.SetState(ScopeState.ShouldBreak);
            scope.SetBreakAmount(breakAmount);
            return FValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString()
        {
            return "BreakNode:";
        }
    }
}
