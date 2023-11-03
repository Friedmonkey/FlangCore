using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class DoubleLiteralNode : SyntaxNode
    {
        public SyntaxToken doubleToken;

        public DoubleLiteralNode(SyntaxToken syntaxToken) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            this.doubleToken = syntaxToken;
        }

        public override NodeType Type => NodeType.IntLiteral;

        public override FValue Evaluate(Scope scope)
        {
            var fdouble = new FDouble((double)doubleToken.Value);
            return fdouble;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(doubleToken);
        }

        public override string ToString()
        {
            return "DoubleLitNode:";
        }
    }
}
