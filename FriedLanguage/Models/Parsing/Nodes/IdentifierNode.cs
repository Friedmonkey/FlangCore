using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class IdentifierNode : SyntaxNode
    {
        public SyntaxToken Token { get; private set; }
        public SyntaxNode IndexerIndex { get; private set; } = null;

        public IdentifierNode(SyntaxToken syntaxToken) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            Token = syntaxToken;
            this.IndexerIndex = null;
        }

        public IdentifierNode(SyntaxToken syntaxToken, SyntaxNode indexerIndex) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            Token = syntaxToken;
            this.IndexerIndex = indexerIndex;
        }

        public override NodeType Type => NodeType.Identifier;

#warning need non null
        public override FValue Evaluate(Scope scope)
        {
            var value = scope.Get(Token.Text);
            if (value == null)
                throw new Exception($"Identifier \"{Token.Text}\" on position {Token.Position} Does not exist!");


            if (this.IndexerIndex != null)
            {
                var index = this.IndexerIndex.Evaluate(scope);
                if (index == null)
                    throw new Exception("Couldnt parse index");

                return value.GetIndex(index);
            } else
                return value;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(Token);
        }

        public override string ToString()
        {
            return "IdentNode:";
        }
    }
}
