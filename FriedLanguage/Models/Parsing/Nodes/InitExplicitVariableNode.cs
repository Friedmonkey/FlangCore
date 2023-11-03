using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class InitExplicitVariableNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode expr;
        private readonly bool isFixedType = true;
        private readonly bool isConst = false;
        private readonly bool isNullable = false;
        private SyntaxToken VarType;

        public InitExplicitVariableNode(SyntaxToken ident, SyntaxToken type, bool isFixedType, bool isConst = false, bool isNullable = false) : base(ident.Position, ident.EndPosition)
        {
            this.ident = ident;
            this.VarType = type;
            this.isFixedType = isFixedType;
            this.isConst = isConst;
            this.isNullable = isNullable;
        }

        public InitExplicitVariableNode(SyntaxToken ident, SyntaxToken type, SyntaxNode expr, bool isFixedType, bool isConst = false, bool isNullable = false) : base(ident.Position, expr.EndPosition)
        {
            this.ident = ident;
            this.VarType = type;
            this.expr = expr;
            this.isFixedType = isFixedType;
            this.isConst = isConst;
            this.isNullable = isNullable;
        }

        public override NodeType Type => NodeType.InitVariable;

        public override FValue Evaluate(Scope scope)
        {
            if (scope.Get(ident.Value.ToString()) != null)
            {
                throw new InvalidOperationException("Can not initiliaze the same variable twice!");
            }

            if (expr != null)
            {
                var val = expr.Evaluate(scope);
                string GotVar = val.BuiltinName.ToString().ToLower();
                if ((GotVar != VarType.Text) && VarType.Text != "object")
                {
                    try
                    {
                        if (Enum.TryParse(VarType.Text, true, out FBuiltinType type))
                            val = val.CastToBuiltin(type);
                        else
                            throw new Exception();
                    }
                    catch 
                    {
                        throw new InvalidOperationException("Tried to initiliaze a "+ VarType.Text + " variable with a "+GotVar+" value; this is not permitted. Use var% or object instead.");
                    }
                }
                val.TypeIsFixed = isFixedType;
                val.IsConstant = isConst;
                val.IsNullable = isNullable;

                scope.Set(ident.Value.ToString(), val);
                return val;
            }
            else
            {
                if (isFixedType)
                {
                    var val = new FClassInstance();
                    switch (VarType.Text)
                    {
                        case "string":
                            val = new FString("");
                            break;
                        case "int":
                            val = new FInt(0);
                            break;
                        case "float":
                            val = new FFloat(0);
                            break;
                        case "double":
                            val = new FDouble(0);
                            break;
                        case "long":
                            val = new FLong(0);
                            break;
                        case "bool":
                            val = new FBool(false);
                            break;
                        case "list":
                            val = new FList();
                            break;
                        case "dictionary":
                            val = new FDictionary();
                            break;
                    }
                    var t = val.GetType();
                    if (val.GetType().Name is "FClassInstance")
                        throw new InvalidOperationException("idk man 2");

                    val.TypeIsFixed = isFixedType;
                    val.IsConstant = isConst;
                    val.IsNullable = isNullable;

                    scope.Set(ident.Value.ToString(), val);
                    return val;
                } 
                var nul = new FNull();
                nul.TypeIsFixed = isFixedType;
                nul.IsConstant = isConst;
                nul.IsNullable = isNullable;

                scope.Set(ident.Value.ToString(), nul);
                return nul;
            }

        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
            if (expr != null) yield return expr;
        }

        public override string ToString()
        {
            return "InitVariableNode:";
        }
    }
}
