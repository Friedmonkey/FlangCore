using FriedLanguage.BuiltinType;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class DictNode : SyntaxNode
    {
        private List<(SyntaxToken tok, SyntaxNode expr)> dict;
        private SyntaxToken lsq;
        private SyntaxToken rsq;

        public DictNode(List<(SyntaxToken tok, SyntaxNode expr)> dict, SyntaxToken lsq, SyntaxToken rsq) : base(lsq.Position, rsq.EndPosition)
        {
            this.dict = dict;
            this.lsq = lsq;
            this.rsq = rsq;
        }

        public override NodeType Type => NodeType.Dict;

        public override FValue Evaluate(Scope scope)
        {
            var dict = new FDictionary();

            foreach (var ent in this.dict)
            {
                dict.Value.Add((new FString(ent.tok.Text), ent.expr.Evaluate(scope)));
            }

            return dict;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            throw new NotImplementedException();
        }
    }
}
