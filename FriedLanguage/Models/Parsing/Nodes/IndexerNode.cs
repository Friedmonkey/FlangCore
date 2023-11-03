using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
	internal class IndexerNode : SyntaxNode
	{
		private SyntaxToken Identifier;
		private SyntaxNode index;

		public IndexerNode(SyntaxToken ident,SyntaxNode index) : base(ident.Position, ident.EndPosition)
		{
			Identifier = ident;
			this.index = index;
		}

		public override NodeType Type => NodeType.Dict;

		public override FValue Evaluate(Scope scope)
		{
			FValue idx = index.Evaluate(scope);

			FValue indexable = scope.Get(Identifier.Text);

            if (indexable == null)
				throw new Exception($"Could not parse.");

			return indexable.GetIndex(idx);
		}

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			throw new NotImplementedException();
		}
	}
}
