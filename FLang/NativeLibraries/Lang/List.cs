using FriedLanguage.BuiltinType;
using FriedLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLang.NativeLibraries
{
    public partial class Lang
    {
        public static class List
        {
            public static FValue Add(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FList self)
                    throw new Exception("Expected argument 0 to be a list");

                if (arguments[1] is not FValue val)
                    throw new Exception("Expected argument 1 to be a value");

                self.Value.Add(val);

                return val;
            }
            public static FValue Remove(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FList self)
                    throw new Exception("Expected argument 0 to be a list");

                if (arguments[1] is not FValue val)
                    throw new Exception("Expected argument 1 to be a value");

                var success = self.Value.Remove(val);

                return new FBool(success);
            }
            public static FValue RemoveAt(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FList self)
                    throw new Exception("Expected argument 0 to be a list");

                if (arguments[1] is not FInt val)
                    throw new Exception("Expected argument 1 to be a int");

                self.Value.RemoveAt(val.Value);

                return val;
            }
            public static FValue Contains(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FList self)
                    throw new Exception("Expected argument 0 to be a list");

                if (arguments[1] is not FValue val)
                    throw new Exception("Expected argument 1 to be a value");

                var success = self.Value.Contains(val);

                return new FBool(success);
            }
            public static FValue Count(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FList self)
                    throw new Exception("Expected argument 0 to be a list");


                return new FInt(self.Value.Count());
            }
            public static FValue First(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FList self)
                    throw new Exception("Expected argument 0 to be a list");


                return self.Value.First();
            }
            public static FValue Last(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FList self)
                    throw new Exception("Expected argument 0 to be a list");


                return self.Value.Last();
            }
        }
    }
}
