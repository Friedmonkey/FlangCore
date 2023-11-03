using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FriedLanguage.BuiltinType
{
    public class FString : FClassInstance
    {
        private static FClass getClass()
        {
            var @class = new FClass("String", true, true);
            return @class;
        }
        public string Value { get; set; }
        public override FBuiltinType BuiltinName => FBuiltinType.String;

        public FString() : base(getClass()) { }
        public FString(string value) : base(getClass())
        {
            Value = value;
        }

        public override FValue Dot(FValue other, SyntaxToken Token = default,Scope scope = default)
        {
            if (other is not FString key) throw NotSupportedBetween(other, "Dot");

            var val = GetValue(key.Value, scope);
            if (Class.Strict && val.IsNull()) throw new Exception($"Property {other.ToSpagString().Value} not on position {Token.Position} found!");

            return val;
        }

		public override FValue GetIndex(FValue index)
		{
			if (index is not FInt fint)
				throw new Exception("Expected int for indexer.");

            return new FString(Value[fint.Value].ToString());
		}

		public override FString ToSpagString()
        {
            return new FString(Value);
        }

        public override string ToString()
        {
            return $"<FString Value={Value}>";
        }

        public override FValue Add(FValue other, Scope scope = null)
        {
            if (other is not FString @string)
            {
                if (other is not FInt @int)
                    throw NotSupportedBetween(other, "Add");
                else
                    return new FString(Value + @int.Value.ToString());
            }
            else
                return new FString(Value + @string.Value);
        }

        public override FValue Idx(FValue other, Scope scope = null)
        {
            if (other is not FInt idx) throw NotSupportedBetween(other, "Add");
            return new FString(Value[idx.Value].ToString());
        }

        public override FValue Equals(FValue other,SyntaxToken callerToken = default, Scope scope = null)
        {
            if ((callerToken.Text == "is" || callerToken.Text == "is not") && other is FClass fclas)
            {
                string name = getClass().Name;
                return new FBool(fclas.Name == name);
            }
            else
            {
                if (other is not FString otherString) return FBool.False;
                return new FBool(Value == otherString.Value);
            }
        }

        public override FValue CastToBuiltin(FBuiltinType other)
        {
            switch (other)
            {
                case FBuiltinType.String:
                    return new FString(this.Value.ToString());
                case FBuiltinType.Int:
                    if (Int32.TryParse(Value, out int ival))
                        return new FInt(ival);
                    else break;
                case FBuiltinType.Float:
                    if (float.TryParse(Value, out float fval))
                        return new FFloat(fval);
                    else break;
                case FBuiltinType.Long:
                    if (long.TryParse(Value, out long lval))
                        return new FLong(lval);
                    else break;
                case FBuiltinType.Double:
                    if (double.TryParse(Value, out double dval))
                        return new FDouble(dval);
                    else break;
                case FBuiltinType.Bool:
                    if (Value == "True")
                        return FBool.True;
                    else if (Value == "False")
                        return FBool.False;
                    else break;
                case FBuiltinType.Null:
                    if (Value == "Null")
                        return FValue.Null;
                    else break;
                default: throw CastInvalid("native " + other.ToString());
            }
            throw new InvalidCastException("Cannot cast string with value "+Value+" to an "+other.ToString());
        }

        public bool SurelyStringEquals(string other)
        {
            return Value == other;
        }

        public override bool IsTruthy()
        {
            return Value != null;
        }
    }
}
