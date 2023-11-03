using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
	internal class GotoNode : SyntaxNode
	{
		public override NodeType Type => NodeType.Break;
		public string Destination { get; protected set; }

		public GotoNode(SyntaxToken gotoToken,string dest) : base(gotoToken.Position, gotoToken.EndPosition) 
		{
			this.Destination = dest;
		}

		public override FValue Evaluate(Scope scope)
		{
			var value = scope.Get(Destination);
			//foreach (var (name,value) in scope.Table) 
			//{
			if (value is FLabel flabel)
			{
				scope.SetState(ScopeState.ShouldJump);
				scope.SetJumpPos(flabel.Position);
			}
			//}

			return FValue.Null;
		}

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			return Enumerable.Empty<SyntaxNode>();
		}

		public override string ToString()
		{
			return "GotoNode:";
		}
	}
}
