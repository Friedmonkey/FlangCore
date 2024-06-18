using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public class FDynamic: FClassInstance
    {
        private static FClass getClass()
        {
            var @class = new FClass("Dynamic", true, true);
            return @class;
        }
        public List<(FValue property, FValue val)> Value { get; set; } = new();
        public override FBuiltinType BuiltinName => FBuiltinType.Dynamic;
        public FDynamic() : base(getClass()) { }
        public FDynamic(List<FValue> Properties, List<FValue> Values) : base(getClass())
        {
            if (Properties.Count() != Values.Count())
                throw new ArgumentException("Keys and Value dont have the same amount of enteries");
            Value.Clear();
            var count = Properties.Count();
            for (int i = 0; i < count; i++)
            {
                Value.Add((Properties[i], Values[i]));
            }
        }

        public override FValue DotAssignment(FValue key, FValue other, Scope scope = null)
        {
            if (key is not FString keyVal) throw NotSupportedBetween(key, "DotAssignment");

            return SetIndex(key, other);
        }

        public override FValue Dot(FValue other, SyntaxToken Token = default, Scope scope = default)
        {
            if (other is not FString key) throw NotSupportedBetween(other, "Dot");

            var val = GetIndex(other);
            //var val = GetValue(key.Value,scope);
            //if (Class.Strict && val.IsNull()) throw new Exception($"Property {other.ToSpagString().Value} not on position {Token.Position} found!");

            return val;
        }

        public override FString ToSpagString()
        {
            return new FString("{\n" + string.Join(",\n  ", Value.Select((v) => v.property.ToSpagString().Value + ": " + v.val.ToSpagString().Value)) + "\n}");
        }

        public override string ToString()
        {
            return $"<FDynamic Value={string.Join(", ", Value)}>";
        }

        public override FValue Idx(FValue other, Scope scope = null)
        {
            foreach (var kvp in Value)
            {
                if (kvp.property.Equals(other).IsTruthy()) return kvp.val;
            }

            return FValue.Null;
        }
        public override FValue SetIndex(FValue index, FValue newValue)
        {
            for (int i = 0; i < Value.Count(); i++)
            {
                var (key, value) = Value[i];
                if (key.Equals(index).IsTruthy())
                {
                    Value[i] = new(key,newValue);
                    return FBool.True;
                }
            }
            //doest exist lets add
            Value.Add(new(index, newValue));
            return FBool.False;
        }
		public override FValue GetIndex(FValue index)
		{
			for (int i = 0; i < Value.Count(); i++)
			{
				var (key, value) = Value[i];
				if (key.Equals(index).IsTruthy())
				{
					return value;
				}
			}
            return FValue.Null;
		}

        public override FValue CastToBuiltin(FBuiltinType other)
        {
            if (other != FBuiltinType.Dictionary)
                throw CastInvalid("native " + other.ToString());


            return new FDictionary(this.Value);
        }

        public override bool IsTruthy()
        {
            return Value != null;
        }
    }
}
