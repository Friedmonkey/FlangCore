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
        public static class Dict
        {
            public static FValue Add(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a list");

                if (arguments[1] is not FValue key)
                    throw new Exception("Expected argument 1 to be a key");

                if (arguments[2] is not FValue val)
                    throw new Exception("Expected argument 2 to be a value");

                self.Value.Add(new(key,val));

                return key;
            }
            public static FValue Get(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a list");

                if (arguments[1] is not FValue key)
                    throw new Exception("Expected argument 1 to be a key");

                var success = false;
                var index = self.Value.FindIndex(a => (a.key.Equals(key) as FBool).Value);
                if (index != -1)
                {
                    success = true;
                    return self.Value.ElementAt(index).val;
                }

                return new FNull();
            }
            public static FValue Remove(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a list");

                if (arguments[1] is not FValue key)
                    throw new Exception("Expected argument 1 to be a key");

                var success = false;
                var index = self.Value.FindIndex(a => (a.key.Equals(key) as FBool).Value);
                if (index != -1)
                {
                    success = true;
                    self.Value.RemoveAt(index);
                }

                return new FBool(success);
            }
            public static FValue RemoveAt(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a list");

                if (arguments[1] is not FInt val)
                    throw new Exception("Expected argument 1 to be a int");

                self.Value.RemoveAt(val.Value);

                return val;
            }
            public static FValue ContainsKey(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a list");

                if (arguments[1] is not FValue key)
                    throw new Exception("Expected argument 1 to be a key");

                var index = self.Value.FindIndex(a => (a.key.Equals(key) as FBool).Value);
                var success = (index != -1);

                return new FBool(success);
            }
            public static FValue ContainsValue(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a list");

                if (arguments[1] is not FValue value)
                    throw new Exception("Expected argument 1 to be a value");

                var index = self.Value.FindIndex(a => (a.val.Equals(value) as FBool).Value);
                var success = (index != -1);

                return new FBool(success);
            }
            public static FValue Count(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a list");


                return new FInt(self.Value.Count());
            }
            public static FValue First(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a list");


                return self.Value.First().val;
            }
            public static FValue Last(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a list");


                return self.Value.Last().val;
            }
        }
    }
}
