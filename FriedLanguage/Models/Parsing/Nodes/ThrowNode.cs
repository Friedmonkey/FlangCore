using FriedLanguage.BuiltinType;
using FriedLanguage.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class ThrowNode : SyntaxNode
    {
        private SyntaxNode expr;

        public ThrowNode(SyntaxNode expr) : base(expr.StartPosition, expr.EndPosition)
        {
            this.Expr = expr;
        }

        public override NodeType Type => NodeType.AssignVariable;

        public SyntaxNode Expr { get => expr; set => expr = value; }

        public override FValue Evaluate(Scope scope)
        {
            var eval = Expr.Evaluate(scope);

            string name = eval.BuiltinName.ToString();

            if (eval is not FClassInstance clas)
                throw new Exception("Expecting Class got " + name);

            if (clas.GetValue("message", scope) is not FString val)
                throw new Exception("message as string not found on " + name);


            throw new Exception(val.Value);
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expr;
        }

        public override string ToString()
        {
            return "ThrowNode:";
        }
    }
}
