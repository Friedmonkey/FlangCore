using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public class FDouble : FClassInstance
    {
        private static FClass getClass()
        {
            var @class = new FClass("Double", true, true);
            return @class;
        }
        public override FBuiltinType BuiltinName => FBuiltinType.Double;
        public double Value { get; set; }

        public FDouble() : base(getClass())
        {
            Value = 0;
        }

        public FDouble(double value) : base(getClass())
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
            if (other is not FDouble otherDouble)
            {
                try
                {
                    otherDouble = (FDouble)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Add on FDouble and " + other.BuiltinName.ToString());
                }
            }
            return new FDouble(Value + otherDouble.Value);
        }

        public override FValue Sub(FValue other, Scope scope = null)
        {
            if (other is not FDouble otherDouble)
            {
                try
                {
                    otherDouble = (FDouble)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Sub on FDouble and " + other.BuiltinName.ToString());
                }
            }
            return new FDouble(Value - otherDouble.Value);
        }

        public override FValue Mul(FValue other, Scope scope = null)
        {
            if (other is not FDouble otherDouble)
            {
                try
                {
                    otherDouble = (FDouble)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Mul on FDouble and " + other.BuiltinName.ToString());
                }
            }
            return new FDouble(Value * otherDouble.Value);
        }

        public override FValue Div(FValue other, Scope scope = null)
        {
            if (other is not FDouble otherDouble)
            {
                try
                {
                    otherDouble = (FDouble)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Div on FDouble and " + other.BuiltinName.ToString());
                }
            }
            return new FDouble(Value / otherDouble.Value);
        }

        public override FValue Mod(FValue other, Scope scope = null)
        {
            if (other is not FDouble otherDouble)
            {
                try
                {
                    otherDouble = (FDouble)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    throw new Exception("Can not perform Mod on FDouble and " + other.BuiltinName.ToString());
                }
            }
            return new FDouble(Value % otherDouble.Value);
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
                if (other is not FDouble otherDouble) throw new Exception("Can not perform EqualsCheck on FDouble and " + other.BuiltinName.ToString());
                return new FBool(Value == otherDouble.Value);
            }
        }

        public override FValue LessThan(FValue other, Scope scope = null)
        {
            if (other is not FDouble otherDouble)
            {
                try
                {
                    otherDouble = (FDouble)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    return FBool.False;
                }
            }
            return new FBool(Value < otherDouble.Value);
        }


        public override FValue LessThanEqu(FValue other, Scope scope = null)
        {
            if (other is not FDouble otherDouble)
            {
                try
                {
                    otherDouble = (FDouble)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    return FBool.False;
                }
            }
            return new FBool(Value <= otherDouble.Value);

        }

        public override FValue GreaterThan(FValue other, Scope scope = null)
        {
            if (other is not FDouble otherDouble)
            {
                try
                {
                    otherDouble = (FDouble)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    return FBool.False;
                }
            }
            return new FBool(Value > otherDouble.Value);

        }

        public override FValue GreaterThanEqu(FValue other, Scope scope = null)
        {
            if (other is not FDouble otherDouble)
            {
                try
                {
                    otherDouble = (FDouble)other.CastToBuiltin(BuiltinName);
                }
                catch
                {
                    return FBool.False;
                }
            }
            return new FBool(Value >= otherDouble.Value);

        }

        public override FValue ArithNot(Scope scope = null)
        {
            return new FDouble(-Value);
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
                    return new FLong((long)Value);
                case FBuiltinType.Float:
                    return new FFloat((float)Value);
                case FBuiltinType.Double:
                    return new FDouble(Value);
                default: throw CastInvalid("native " + other.ToString());
            }
        }

        public override bool IsTruthy()
        {
            return Value == 1;
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
