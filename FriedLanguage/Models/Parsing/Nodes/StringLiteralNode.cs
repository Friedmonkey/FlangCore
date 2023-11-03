using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class StringLiteralNode : SyntaxNode
    {
        private SyntaxToken syntaxToken;
        private SyntaxNode indexerIndex = null;


        public StringLiteralNode(SyntaxToken syntaxToken) : base(syntaxToken.Position, syntaxToken.EndPosition)
        {
            this.syntaxToken = syntaxToken;
            this.indexerIndex = null;
        }
		public StringLiteralNode(SyntaxToken syntaxToken,SyntaxNode indexerIndex) : base(syntaxToken.Position, syntaxToken.EndPosition)
		{
			this.syntaxToken = syntaxToken;
            this.indexerIndex = indexerIndex;
		}

		public override NodeType Type => NodeType.StringLiteral;

        public override FValue Evaluate(Scope scope)
        {
            var value = new FString((string)syntaxToken.Value);

			if (this.indexerIndex != null)
			{
				var index = this.indexerIndex.Evaluate(scope);
				if (index == null)
					throw new Exception("Couldnt parse index");

				return value.GetIndex(index);
			}
			else
				return value;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(syntaxToken);
        }

        public override string ToString()
        {
            return "StringLitNode:";
        }
    }
}
