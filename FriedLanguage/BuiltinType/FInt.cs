using FriedLanguage.BuiltinType;
using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public class FInt : FClassInstance
    {
        private static FClass getClass()
        {
            var @class = new FClass("Int", true, true);
            return @class;
        }
        public override FBuiltinType BuiltinName => FBuiltinType.Int;
        public int Value { get; set; }

        public FInt() : base(getClass())
        {
            Value = 0;
        }

        public FInt(int value) : base(getClass())
        {
            Value = value;
        }
		public FInt(bool value) : base(getClass())
		{
			Value = value?1:0;
		}
		public override FValue Dot(FValue other, SyntaxToken Token = default, Scope scope = default)
        {
            if (other is not FString key) throw NotSupportedBetween(other, "Dot");

            var val = GetValue(key.Value, scope);
            if (Class.Strict && val.IsNull()) throw new Exception($"Property {other.ToSpagString().Value} not on position {Token.Position} found!");

            return val;
        }

        public override FValue Add(FValue other, Scope scope = null)
        {
            if (other is not FInt otherInt)
            {
                try
                {
                    otherInt = (FInt)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Add on FInt and " + other.BuiltinName.ToString());
                }
            }

            return new FInt(Value + otherInt.Value);
        }

        public override FValue Sub(FValue other, Scope scope = null)
        {
            if (other is not FInt otherInt)
            {
                try
                {
                    otherInt = (FInt)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Sub on FInt and " + other.BuiltinName.ToString());
                }
            }
            return new FInt(Value - otherInt.Value);
        }

        public override FValue Mul(FValue other, Scope scope = null)
        {
            if (other is not FInt otherInt)
            {
                try
                {
                    otherInt = (FInt)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Mul on FInt and " + other.BuiltinName.ToString());
                }
            }
            return new FInt(Value * otherInt.Value);
        }

        public override FValue Div(FValue other, Scope scope = null)
        {
            if (other is not FInt otherInt)
            {
                try
                {
                    otherInt = (FInt)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Div on FInt and " + other.BuiltinName.ToString());
                }
            }
            return new FInt(Value / otherInt.Value);
        }

        public override FValue Mod(FValue other, Scope scope = null)
        {
            if (other is not FInt otherInt)
            {
                try
                {
                    otherInt = (FInt)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Mod on FInt and " + other.BuiltinName.ToString());
                }
            }
            return new FInt(Value % otherInt.Value);
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
                if (other is not FInt otherInt) return FBool.False;
                return new FBool(Value == otherInt.Value);
            }
        }

        public override FValue LessThan(FValue other, Scope scope = null)
        {
            if (other is not FInt otherInt)
            {
                try
                {
                    otherInt = (FInt)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    return FBool.False;
                }
            }
            return new FBool(Value < otherInt.Value);
        }

        public override FValue LessThanEqu(FValue other, Scope scope = null)
        {
            if (other is not FInt otherInt)
            {
                try
                {
                    otherInt = (FInt)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    return FBool.False;
                }
            }
            return new FBool(Value <= otherInt.Value);

        }

        public override FValue GreaterThan(FValue other, Scope scope = null)
        {
            if (other is not FInt otherInt)
            {
                try
                {
                    otherInt = (FInt)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    return FBool.False;
                }
            }
            return new FBool(Value > otherInt.Value);

        }

        public override FValue GreaterThanEqu(FValue other, Scope scope = null)
        {
            if (other is not FInt otherInt)
            {
                try
                {
                    otherInt = (FInt)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    return FBool.False;
                }
            }
            return new FBool(Value >= otherInt.Value);

        }
        public override FValue ArithNot(Scope scope = null)
        {
            return new FInt(-Value);
        }

        public override FValue CastToBuiltin(FBuiltinType other)
        {
            switch (other)
            {
                case FBuiltinType.String:
                    return new FString(this.Value.ToString());
                case FBuiltinType.Int:
                    return new FInt(Value);
                case FBuiltinType.Long:
                    return new FLong(Value);
                case FBuiltinType.Float:
                    return new FFloat(Value);
                case FBuiltinType.Double:
                    return new FDouble(Value);
                default: throw CastInvalid("native " + other.ToString());
            }
        }

        public override bool IsTruthy()
        {
            return Value == 1;
        }

        public override FValue Not(Scope scope = null)
        {
            return ArithNot();
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
