using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class LongLiteralNode : SyntaxNode
    {
        public SyntaxToken longToken;

        public LongLiteralNode(SyntaxToken syntaxToken) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            this.longToken = syntaxToken;
        }

        public override NodeType Type => NodeType.IntLiteral;

        public override FValue Evaluate(Scope scope)
        {
            var flong = new FLong((long)longToken.Value);
            return flong;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(longToken);
        }

        public override string ToString()
        {
            return "LongLitNode:";
        }
    }
}
