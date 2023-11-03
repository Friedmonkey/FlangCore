using FriedLanguage.BuiltinType;
using FriedLanguage.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class InstantiateNode : SyntaxNode
    {
        private SyntaxToken ident;
        private List<SyntaxNode> argumentNodes;

        public InstantiateNode(SyntaxToken ident, List<SyntaxNode> argumentNodes) : base(ident.Position, argumentNodes.GetEndingPosition(ident.EndPosition))
        {
            this.ident = ident;
            this.argumentNodes = argumentNodes;
        }

        public override NodeType Type => NodeType.Instantiate;

        public override FValue Evaluate(Scope scope)
        {
            var @class = scope.Get(ident.Text);
            if (@class == null || @class is not FClass sclass) throw new Exception("Class " + ident.Text + " not found!");


            var instance = new FClassInstance(sclass);

            List<FValue> args = new() { instance };
            foreach (var n in argumentNodes) args.Add(n.Evaluate(scope));

            instance.CallConstructor(scope, args);

            return instance;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
            foreach (var n in argumentNodes) yield return n;
        }
    }
}
