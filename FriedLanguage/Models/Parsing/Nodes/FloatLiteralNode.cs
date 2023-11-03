using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class FloatLiteralNode : SyntaxNode
    {
        private SyntaxToken syntaxToken;

        public FloatLiteralNode(SyntaxToken syntaxToken) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            this.syntaxToken = syntaxToken;
        }

        public override NodeType Type => NodeType.FloatLiteral;

        public override FValue Evaluate(Scope scope)
        {
            return new FFloat((float)syntaxToken.Value);
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(syntaxToken);
        }

        public override string ToString()
        {
            return "FloatLitNode:";
        }
    }
}
