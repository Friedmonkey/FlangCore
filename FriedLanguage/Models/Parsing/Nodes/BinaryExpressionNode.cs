using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class BinaryExpressionNode : SyntaxNode
    {
        private SyntaxNode left;
        private SyntaxToken operatorToken;
        private SyntaxNode right;

        public BinaryExpressionNode(SyntaxNode left, SyntaxToken operatorToken, SyntaxNode right) : base(left.StartPosition, right.EndPosition)
        {
            this.left = left;
            this.operatorToken = operatorToken;
            this.right = right;
        }

        public override NodeType Type => NodeType.BinaryExpression;

        public override FValue Evaluate(Scope scope)
        {
            var leftRes = left.Evaluate(scope);
            var rightRes = right.Evaluate(scope);

            switch (operatorToken.Type)
            {
                case SyntaxType.Plus:
                    return leftRes.Add(rightRes,scope);
                case SyntaxType.Minus:
                    return leftRes.Sub(rightRes, scope);
                case SyntaxType.Div:
                    return leftRes.Div(rightRes, scope);
                case SyntaxType.Mul:
                    return leftRes.Mul(rightRes, scope);
                case SyntaxType.Mod:
                    return leftRes.Mod(rightRes, scope);
                case SyntaxType.EqualsEquals:
                    return leftRes.Equals(rightRes, operatorToken, scope);
                case SyntaxType.BangEquals:
                    return leftRes.Equals(rightRes, operatorToken, scope).Not(scope);
                case SyntaxType.Idx:
                    return leftRes.Idx(rightRes, scope);
                case SyntaxType.LessThan:
                    return leftRes.LessThan(rightRes, scope);
                case SyntaxType.LessThanEqu:
                    return leftRes.LessThanEqu(rightRes, scope);
                case SyntaxType.GreaterThan:
                    return leftRes.GreaterThan(rightRes, scope);
                case SyntaxType.GreaterThanEqu:
                    return leftRes.GreaterThanEqu(rightRes, scope);
                case SyntaxType.AndAnd:
                    return new FInt((leftRes.IsTruthy() && rightRes.IsTruthy()) ? 1 : 0);
                case SyntaxType.OrOr:
                    return new FInt((leftRes.IsTruthy() || rightRes.IsTruthy()) ? 1 : 0);
                default:
                    throw new NotImplementedException($"Operator {operatorToken.Type} does not have an implementation for binary expressions.");
            }
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return left;
            yield return new TokenNode(operatorToken);
            yield return right;
        }

        public override string ToString()
        {
            return "BinaryExprNode: op=" + operatorToken.Type;
        }
    }
}
