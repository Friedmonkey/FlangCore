using FriedLanguage.BuiltinType;
using FriedLanguage.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class CallNode : SyntaxNode
    {
        public SyntaxNode ToCallNode { get; set; }
        private List<SyntaxNode> argumentNodes;
		public SyntaxNode IndexerIndex { get; private set; } = null;


		public CallNode(SyntaxNode atomNode, List<SyntaxNode> argumentNodes) : base(atomNode.StartPosition, argumentNodes.GetEndingPosition(atomNode.EndPosition))
        {
            ToCallNode = atomNode;
            this.argumentNodes = argumentNodes;
            this.IndexerIndex = null;
        }
		public CallNode(SyntaxNode atomNode, List<SyntaxNode> argumentNodes,SyntaxNode indexerIndex) : base(atomNode.StartPosition, argumentNodes.GetEndingPosition(atomNode.EndPosition))
		{
			ToCallNode = atomNode;
			this.argumentNodes = argumentNodes;
			this.IndexerIndex = indexerIndex;
		}

		public override NodeType Type => NodeType.Call;

        public override FValue Evaluate(Scope scope)
        {
            SyntaxToken token = default;
            if (ToCallNode is IdentifierNode ident)
                token = ident.Token;
            var toCall = ToCallNode.Evaluate(scope) ?? FValue.Null;
            var args = EvaluateArgs(scope);

            var value = toCall.Call(scope, args, token);


			if (this.IndexerIndex != null)
			{
				var index = this.IndexerIndex.Evaluate(scope);
				if (index == null)
					throw new Exception("Couldnt parse index");

				return value.GetIndex(index);
			}
			else
				return value;
        }

        public List<FValue> EvaluateArgs(Scope scope)
        {
            var args = new List<FValue>();

            foreach (var n in argumentNodes) args.Add(n.Evaluate(scope));
            return args;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ToCallNode;
            foreach (var n in argumentNodes) yield return n;
        }

        public override string ToString()
        {
            return "CallNode:";
        }
    }
}
