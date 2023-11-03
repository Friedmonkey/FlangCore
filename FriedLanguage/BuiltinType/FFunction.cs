using FriedLanguage.BuiltinType;
using FriedLanguage.Models;
using FriedLanguage.Models.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public class FFunction : FBaseFunction
    {
        public override FBuiltinType BuiltinName => FBuiltinType.Function;
        public string FunctionName { get; set; }
        public SyntaxNode Callback { get; set; }
        public Scope DefiningScope { get; set; }


        public FFunction(Scope definingScope, string functionName, List<string> args, List<string> argTypes,string returnType, SyntaxNode callback)
        {
            DefiningScope = definingScope;
            FunctionName = functionName;
            ExpectedArgs = args;
            ExpectedArgTypes = argTypes;
            ReturnType = returnType;
            Callback = callback;

            // If the scope is the global scope, we need to clone it as otherwise we would have a reference to the global scope
            // and any changes to the scope would be reflected in the global scope
            if (DefiningScope.ParentScope == null)
            {
                DefiningScope = DefiningScope.Clone();
            }
        }

        public override FValue Call(Scope scope, List<FValue> args,SyntaxToken token = default)
        {
            if (args.Count != ExpectedArgs.Count) throw new Exception(FunctionName + " expected " + ExpectedArgs.Count + " arguments. (" + string.Join(", ", ExpectedArgs) + ")");

            Scope funcScope = new(DefiningScope, DefiningScope.CreatedPosition);

            for (int i = 0; i < ExpectedArgs.Count; i++)
            {
                if (!((args[i].BuiltinName.ToString().ToLower() == ExpectedArgTypes[i]) || ExpectedArgTypes[i] == "object"))
                    throw new Exception($"Expected {ExpectedArgTypes[i]} for {ExpectedArgs[i]} got {args[i].BuiltinName.ToString().ToLower()}");
                funcScope.Set(ExpectedArgs[i], args[i]);
            }

            Callback.Evaluate(funcScope);
            scope.SetState(ScopeState.None);


            var returned = funcScope.ReturnValue.BuiltinName.ToString().ToLower();
            bool MathingReturn = (returned == ReturnType);
            bool nullVoidCase = (returned == "null" && ReturnType == "void");
            bool objectCase = (ReturnType == "object");


            if (!(MathingReturn || nullVoidCase || objectCase))
            {
                throw new Exception($"{FunctionName} was expected to return a {ReturnType}, but instead returned {returned}");
            }

            return funcScope.ReturnValue;
        }

        public override bool IsTruthy()
        {
            return true;
        }
    }
}
