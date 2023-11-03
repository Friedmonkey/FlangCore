using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes 
{ 
    public class TokenNode : SyntaxNode
    {
        public override NodeType Type => NodeType.Token;
        public SyntaxToken Token { get; set; }

        public TokenNode(SyntaxToken token) : base(token.Position, token.EndPosition)
        {
            Token = token;
        }

        public override FValue Evaluate(Scope scope)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override string ToString()
        {
            return "TokenNode: " + Token.Type.ToString() + " val=" + Token.Value?.ToString() + " text=" + Token.Text.ToString();
        }
    }
}
