using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class ClassPropDefinitionNode : SyntaxNode
    {
        public SyntaxToken Name { get; }
        public SyntaxToken VarType { get; }
        public SyntaxNode Expression { get; }
        public bool IsStatic { get; }
        public bool FixedType { get; }
        public bool IsConstant { get; }
        public bool IsNullable { get; }

        public ClassPropDefinitionNode(SyntaxToken name, SyntaxToken varType, SyntaxNode expr, bool isStatic, bool fixedType, bool isConstand, bool isNullable) : base(name.Position, (expr is null) ? name.EndPosition : expr.EndPosition)
        {
            this.Name = name;
            this.VarType = varType;
            this.Expression = expr;
            this.IsStatic = isStatic;
            this.FixedType = fixedType;
            this.IsConstant = isConstand;
            this.IsNullable = isNullable;
        }

        public override NodeType Type => NodeType.ClassPropertyDefinition;

        public override FValue Evaluate(Scope scope)
        {
            throw new NotImplementedException("This should not be called!");
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(Name);
            yield return Expression;
        }
    }
}
