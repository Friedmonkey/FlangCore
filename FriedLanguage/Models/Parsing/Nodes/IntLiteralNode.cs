using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class IntLiteralNode : SyntaxNode
    {
        public SyntaxToken intToken;

        public IntLiteralNode(SyntaxToken syntaxToken) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            this.intToken = syntaxToken;
        }

        public override NodeType Type => NodeType.IntLiteral;

        public override FValue Evaluate(Scope scope)
        {
            var fint = new FInt((int)intToken.Value);
            return fint;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(intToken);
        }

        public override string ToString()
        {
            return "IntLitNode:";
        }
    }
}
