using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class CastNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode node;
        public FBuiltinType type;

        public CastNode(SyntaxToken ident, SyntaxNode node) : base(ident.Position, node.EndPosition)
        {
            this.ident = ident;
            this.node = node;

            // TODO: Allow for cast to classes
            if (!Enum.TryParse<FBuiltinType>(ident.Text, true, out type)) throw new Exception("Unknown type " + ident.Text + "; only builtin types supported right now.");
        }

        public override NodeType Type => NodeType.Cast;

        public override FValue Evaluate(Scope scope)
        {
            return node.Evaluate(scope).CastToBuiltin(type);
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
            yield return node;
        }
    }
}
