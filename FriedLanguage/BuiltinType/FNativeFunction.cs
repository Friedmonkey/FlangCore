using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
	public class FNativeFunction : FBaseFunction
	{
		public override FBuiltinType BuiltinName => FBuiltinType.NativeFunc;
		public Func<Scope, List<FValue>, FValue> Impl { get; set; }

		public FNativeFunction(Func<Scope, List<FValue>, FValue> impl)
		{
			Impl = impl;
			ExpectedArgs = new();
		}

		public FNativeFunction(Func<Scope, List<FValue>, FValue> impl, List<string> expectedArgs)
		{
			Impl = impl;
			ExpectedArgs = expectedArgs;
		}

		public FNativeFunction(Func<Scope, List<FValue>, FValue> impl, List<string> expectedArgs, bool isClassInstanceFunc = false)
		{
			Impl = impl;
			ExpectedArgs = expectedArgs;
			IsClassInstanceMethod = isClassInstanceFunc;
		}

		/// <summary>
		/// NOTE: The scope in SNativeFunction is the calling scope, but not in SFunction!
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public override FValue Call(Scope scope, List<FValue> args, SyntaxToken token = default)
		{
			if (args.Count != ExpectedArgs.Count) throw new Exception("Expected " + ExpectedArgs.Count + " arguments. (" + string.Join(", ", ExpectedArgs) + ")");

			return Impl(scope, args);
		}

        public override bool IsTruthy()
		{
			return true;
		}
	}
}
