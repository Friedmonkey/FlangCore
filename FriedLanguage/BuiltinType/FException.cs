using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public class FException : FClassInstance
    {
        private static FClass getClass()
        {
            var @class = new FClass("Exception", true, true);
            return @class;
        }
        public string Value { get; set; }
        public string Message { get; set; }
        public override FBuiltinType BuiltinName => FBuiltinType.Exception;

        public FException() : base(getClass()) { }
        public FException(string value) : base(getClass())
        {
            Value = value;
        }

        public override FValue Dot(FValue other, SyntaxToken Token = default, Scope scope = default)
        {
            if (other is not FString key) throw NotSupportedBetween(other, "Dot");

            var val = GetValue(key.Value, scope);

            if (val is FNull && key.Value == "message")
                val = new FString(Message);

            if (Class.Strict && val.IsNull()) throw new Exception($"Property {other.ToSpagString().Value} not on position {Token.Position} found!");

            return val;
        }

        public override FString ToSpagString()
        {
            return new FString(Value);
        }

        public override string ToString()
        {
            return $"<FException Value={Value}>";
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

        public override FValue Equals(FValue other, SyntaxToken callerToken = default, Scope scope = null)
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
