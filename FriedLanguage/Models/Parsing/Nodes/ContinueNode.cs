using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class ContinueNode : SyntaxNode
    {
        public override NodeType Type => NodeType.Continue;

        public ContinueNode(SyntaxToken continueToken) : base(continueToken.Position, continueToken.EndPosition) { }

        public override FValue Evaluate(Scope scope)
        {
            scope.SetState(ScopeState.ShouldContinue);
            return FValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString()
        {
            return "ContinueNode:";
        }
    }
}
