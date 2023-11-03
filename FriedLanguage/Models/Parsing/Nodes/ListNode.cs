using FriedLanguage.BuiltinType;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class ListNode : SyntaxNode
    {
        private List<SyntaxNode> list;

        public ListNode(List<SyntaxNode> list, SyntaxToken lsqBracket, SyntaxToken rsqBracket) : base(lsqBracket.Position, rsqBracket.EndPosition)
        {
            this.list = list;
        }

        public override NodeType Type => NodeType.List;

        public override FValue Evaluate(Scope scope)
        {
            FList sList = new();

            foreach (var n in list)
            {
                sList.Value.Add(n.Evaluate(scope));
            }

            return sList;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var n in list) yield return n;
        }

        public override string ToString()
        {
            return "ListNode:";
        }
    }
}
