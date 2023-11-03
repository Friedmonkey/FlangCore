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
    public class FNull : FClassInstance
    {
        private static FClass getClass()
        {
            var @class = new FClass("Null", true, true);
            return @class;
        }
        public FNull() : base(getClass()) { }

        public override FBuiltinType BuiltinName => FBuiltinType.Null;

        public override FValue Dot(FValue other, SyntaxToken Token = default, Scope scope = default)
        {
            if (other is not FString key) throw NotSupportedBetween(other, "Dot");

            var val = GetValue(key.Value, scope);
            if (Class.Strict && val.IsNull()) throw new Exception($"Property {other.ToSpagString().Value} not on position {Token.Position} found!");

            return val;
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
                return new FBool(true);
                //throw base.NotSupportedOn("Equals");
            }
        }

        public override bool IsNull()
        {
            return true;
        }

        public override bool IsTruthy()
        {
            return false;
        }

        public override FString ToSpagString()
        {
            return new("Null");
        }

        public override string ToString()
        {
            return "<FNull>";
        }
    }
}
