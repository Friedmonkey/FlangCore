using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class ExportNode : SyntaxNode
    {
        private SyntaxToken ident;
        public bool Extend { get; protected set; } = false;

        public ExportNode(SyntaxToken ident, bool extend = false) : base(ident.Position, ident.EndPosition)
        {
            this.ident = ident;
            this.Extend = extend;
        }

        public override NodeType Type => NodeType.Export;

        public override FValue Evaluate(Scope scope)
        {
            var val = scope.Get(ident.Text);
            if (val == null)
            {
                if (ident.Text is "scope" or "all" or "self" or "this")
                {
                    foreach (var (key,value) in scope.Table)
                    {
                        scope.GetRoot().ExportTable.Add(key,new(value,Extend));
                    }
                    return FValue.Null;
                }
                else
                    throw new Exception("Can not export value of non-existent identifier");
            }
                scope.GetRoot().ExportTable.Add(ident.Text, new (val,Extend));
            return val;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
        }
    }
}
