using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class ReturnNode : SyntaxNode
    {
        public ReturnNode(SyntaxToken tok) : base(tok.Position, tok.EndPosition)
        {
        }

        public ReturnNode(SyntaxToken tok, SyntaxNode returnValueNode) : base(tok.Position, returnValueNode.EndPosition)
        {
            ReturnValueNode = returnValueNode;
        }

        public SyntaxNode ReturnValueNode { get; }

        public override NodeType Type => NodeType.Return;

        public override FValue Evaluate(Scope scope)
        {
            if (ReturnValueNode != null)
            {
                var v = ReturnValueNode.Evaluate(scope);
                scope.SetReturnValue(v);
            }

            scope.SetState(ScopeState.ShouldReturn);

            return FValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (ReturnValueNode == null) yield break;
            else yield return ReturnValueNode;
        }

        public override string ToString()
        {
            return "ReturnNode:";
        }
    }
}
