using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
	public class FLabel : FClassInstance
	{
		private static FClass getClass()
		{
			var @class = new FClass("Label", true, true);
			return @class;
		}
		public int Position { get; set; }
		public override FBuiltinType BuiltinName => FBuiltinType.Label;

		public FLabel() : base(getClass()) { }
		public FLabel(int pos) : base(getClass())
		{
			this.Position = pos;
		}

		public override FValue Dot(FValue other, SyntaxToken Token = default, Scope scope = default)
		{
			if (other is not FString key) throw NotSupportedBetween(other, "Dot");

			var val = GetValue(key.Value, scope);
			if (Class.Strict && val.IsNull()) throw new Exception($"Property {other.ToSpagString().Value} not on position {Token.Position} found!");

			return val;
		}

		public override FString ToSpagString()
		{
			return new FString("Label:"+Position);
		}

		public override string ToString()
		{
			return $"<FLabel Pos={Position}>";
		}
	}
}
