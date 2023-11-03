using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public class FList : FClassInstance
    {
        private static FClass getClass()
        {
            var @class = new FClass("List", true, true);
            return @class;
        }
        public List<FValue> Value { get; set; } = new();
        public override FBuiltinType BuiltinName => FBuiltinType.List;

        public FList() : base(getClass()) { }

        public FList(byte[] array) : base(getClass())
        {
            Value = new List<FValue>();

            foreach (var val in array)
                Value.Add(new FInt(val));
        }
        public FList(params string[] vals) : base(getClass())
        {
            Value = new List<FValue>();
            foreach (var val in vals)
            {
                Value.Add(new FString(val));
            }
        }
        public FList(params FValue[] vals) : base(getClass())
        {
            Value = vals.ToList();
        }

        public override FValue Dot(FValue other, SyntaxToken Token = default, Scope scope = default)
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

            return Value.ElementAt(fint.Value);
		}
		public override FValue SetIndex(FValue index, FValue newValue)
		{
			if (index is not FInt fint)
				throw new Exception("Expected int for indexer.");

			return Value[fint.Value] = newValue;
		}


		public override FString ToSpagString()
        {
            return new FString("[" + string.Join(", ", Value.Select((v) => v.ToSpagString().Value)) + "]");
        }

        public override string ToString()
        {
            return $"<FString Value={string.Join(", ", Value)}>";
        }

        public override FValue Idx(FValue other, Scope scope = null)
        {
            if (other is not FInt otherInt) throw new Exception("Can only index SList with integers, got " + other.BuiltinName.ToString());

            if (otherInt.Value < 0 || otherInt.Value > Value.Count - 1) throw new Exception("Out of bounds access. SList had " + Value.Count + " elements, but index " + otherInt.Value + " was accessed");
            return Value[otherInt.Value];
        }

        public override FValue Add(FValue other, Scope scope = null)
        {
            Value.Add(other);
            return this;
        }

        public override FValue Sub(FValue other, Scope scope = null)
        {
            if (other is not FInt otherInt) throw new Exception("Can only index SList with integers, got " + other.BuiltinName.ToString());

            if (otherInt.Value < 0 || otherInt.Value > Value.Count - 1) throw new Exception("Out of bounds access. SList had " + Value.Count + " elements, but index " + otherInt.Value + " was accessed");
            Value.RemoveAt(otherInt.Value);

            return this;
        }

        public override bool IsTruthy()
        {
            return Value != null;
        }
    }
}
