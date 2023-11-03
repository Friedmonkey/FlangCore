using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public class FBool : FClassInstance
    {
        private static FClass getClass()
        {
            var @class = new FClass("Bool", true, true);
            return @class;
        }
        public override FBuiltinType BuiltinName => FBuiltinType.Bool;
        public bool Value { get; set; }

        public static FBool False => new FBool(false);
        public static FBool True => new FBool(true);

        public FBool() : base(getClass())
        {
            Value = false;
        }

        public FBool(bool value) : base(getClass())
        {
            Value = value;
        }
        public FBool(int value) : base(getClass())
        {
            Value = value==1 ? true : false;
        }
        public override FValue Dot(FValue other, SyntaxToken Token = default, Scope scope = default)
        {
            if (other is not FString key) throw NotSupportedBetween(other, "Dot");

            var val = GetValue(key.Value,scope);
            if (Class.Strict && val.IsNull()) throw new Exception($"Property {other.ToSpagString().Value} not on position {Token.Position} found!");

            return val;
        }

        public override FValue Add(FValue other, Scope scope = null)
        {
            if (other is not FBool otherBool) throw new Exception("Can not perform Add on SInt and " + other.BuiltinName.ToString());

            return new FBool((Value && otherBool.Value));
        }
        public override FValue Equals(FValue other, SyntaxToken callerToken = default, Scope scope = null)
        {
            if ((callerToken.Text == "is" || callerToken.Text == "is not") && other is FClass fclas)
            {
                string name = getClass().Name;
                return new FBool(fclas.Name == name);
            }
            else
            { 
                if (other is not FBool otherBool) return False;
                return Value == otherBool.Value ? True : False;
            }
        }


        public override FValue CastToBuiltin(FBuiltinType other)
        {
            switch (other)
            {
                case FBuiltinType.Bool:
                    return new FBool(this.Value);
                case FBuiltinType.Int:
                    return new FInt(Value);
                case FBuiltinType.String:
                    return new FString(Value?"True":"False");
                default: throw CastInvalid("native " + other.ToString());
            }
        }

        public override bool IsTruthy()
        {
            return Value;
        }

        public override FValue Not(Scope scope = null)
        {
            return new FBool(!Value);
        }

        public override string ToString()
        {
            return $"<{BuiltinName.ToString()} value={Value}>";
        }

        public override FString ToSpagString()
        {
            return new FString(Value.ToString());
        }
    }
}
