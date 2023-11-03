using FriedLanguage.BuiltinType;
using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public class FLong : FClassInstance
    {
        private static FClass getClass()
        {
            var @class = new FClass("Long", true, true);
            return @class;
        }
        public override FBuiltinType BuiltinName => FBuiltinType.Long;
        public long Value { get; set; }

        public static FLong Zero => new FLong(0);
        public static FLong One => new FLong(1);

        public FLong() : base(getClass())
        {
            Value = 0;
        }

        public FLong(long value) : base(getClass())
        {
            Value = value;
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
            if (other is not FLong otherLong)
            {
                try
                {
                    otherLong = (FLong)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Add on FLong and " + other.BuiltinName.ToString());
                }
            }
            return new FLong(Value + otherLong.Value);
        }

        public override FValue Sub(FValue other, Scope scope = null)
        {
            if (other is not FLong otherLong)
            {
                try
                {
                    otherLong = (FLong)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Sub on FLong and " + other.BuiltinName.ToString());
                }
            }
            return new FLong(Value - otherLong.Value);
        }

        public override FValue Mul(FValue other, Scope scope = null)
        {
            if (other is not FLong otherLong)
            {
                try
                {
                    otherLong = (FLong)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Mul FLong and " + other.BuiltinName.ToString());
                }
            }
            return new FLong(Value * otherLong.Value);
        }

        public override FValue Div(FValue other, Scope scope = null)
        {
            if (other is not FLong otherLong)
            {
                try
                {
                    otherLong = (FLong)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Div FLong and " + other.BuiltinName.ToString());
                }
            }
            return new FLong(Value / otherLong.Value);
        }

        public override FValue Mod(FValue other, Scope scope = null)
        {
            if (other is not FLong otherLong)
            {
                try
                {
                    otherLong = (FLong)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Mod FLong and " + other.BuiltinName.ToString());
                }
            }
            return new FLong(Value % otherLong.Value);
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
                if (other is not FLong otherLong) return FBool.False;
                return new FBool(Value == otherLong.Value);
            }
        }

        public override FValue LessThan(FValue other, Scope scope = null)
        {
            if (other is not FLong otherLong)
            {
                try
                {
                    otherLong = (FLong)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    return FBool.False;
                }
            }
            return new FBool(Value < otherLong.Value);
        }

        public override FValue LessThanEqu(FValue other, Scope scope = null)
        {
            if (other is not FLong otherLong)
            {
                try
                {
                    otherLong = (FLong)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    return FBool.False;
                }
            }
            return new FBool(Value <= otherLong.Value);

        }

        public override FValue GreaterThan(FValue other, Scope scope = null)
        {
            if (other is not FLong otherLong)
            {
                try
                {
                    otherLong = (FLong)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    return FBool.False;
                }
            }
            return new FBool(Value > otherLong.Value);
        }

        public override FValue GreaterThanEqu(FValue other, Scope scope = null)
        {
            if (other is not FLong otherLong)
            {
                try
                {
                    otherLong = (FLong)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    return FBool.False;
                }
            }
            return new FBool(Value >= otherLong.Value);
        }

        public override FValue ArithNot(Scope scope = null)
        {
            return new FLong(-Value);
        }

        public override FValue CastToBuiltin(FBuiltinType other)
        {
            switch (other)
            {
                case FBuiltinType.String:
                    return new FString(this.Value.ToString());
                case FBuiltinType.Int:
                    return new FInt((int)Value);
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
