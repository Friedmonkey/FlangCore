using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class AssignVariableNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode expr;
        private SyntaxNode indexExpr = null;
        private SyntaxNode NullExpr = null;

        public AssignVariableNode(SyntaxToken ident, SyntaxNode expr) : base(ident.Position, expr.EndPosition)
        {
            this.Ident = ident;
            this.Expr = expr;
        }
        public AssignVariableNode(SyntaxToken ident, SyntaxNode idxExpr, SyntaxNode expr, SyntaxNode nullExpr) : base(ident.Position, expr.EndPosition)
        {
            this.Ident = ident;
            this.indexExpr = idxExpr;
            this.Expr = expr;
            this.NullExpr = nullExpr;
        }

        public override NodeType Type => NodeType.AssignVariable;

        public SyntaxToken Ident { get => ident; set => ident = value; }
        public SyntaxNode Expr { get => expr; set => expr = value; }
        public SyntaxNode IdxExpr { get => indexExpr; set => indexExpr = value; }

        public override FValue Evaluate(Scope scope)
        {
            if (scope.Get(Ident.Value.ToString()) == null)
            {
                throw new InvalidOperationException("Can not assign to a non-existant identifier");
            }

            var val = Expr.Evaluate(scope);
            var key = Ident.Value.ToString();

            if (val is FNull && NullExpr != null)
            {
                var nullExpr = NullExpr.Evaluate(scope);
                val = nullExpr;
            }

            if (IdxExpr != null)
            { 
                var index = IdxExpr.Evaluate(scope);
                if (!scope.UpdateIndex(key,index, val, out Exception ex)) throw ex;
                return val;
            }

            if (!scope.Update(key, val, out Exception ex3)) throw ex3;
            return val;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(Ident);
            yield return Expr;
        }

        public override string ToString()
        {
            return "AssignVariableNode:";
        }
    }
}
